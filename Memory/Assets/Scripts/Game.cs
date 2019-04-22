using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

public enum GameStatus
{
    waiting_on_first_card = 0,
    waiting_on_second_card,
    match_found,
    no_match_found
}

public class Game : MonoBehaviour {

    [SerializeField] private int columns;
    [SerializeField] private int rows;
    [SerializeField] float totalPairs;

    [SerializeField] private string frontDirectory = "Sprites/Front/";
    [SerializeField] private string backDirectory = "Sprites/Back/";

    [SerializeField] private Sprite[] fronts;
    [SerializeField] private Sprite[] backs;

    [SerializeField] List<Sprite> frontSprites;
    [SerializeField] private Sprite backSprite;

    [SerializeField] private GameObject cardPrefab;

    [SerializeField] Stack<GameObject> stackOfCards;
    [SerializeField] private GameObject[,] placedCards;

    [SerializeField] private GameObject fieldAnchor;
    [SerializeField] private float x_Offset;
    [SerializeField] private float y_Offset;

    [SerializeField] private GameObject[] selectedCards;
    [SerializeField] private GameStatus status;

    [SerializeField] private float timeoutTargetTime;
    private float timeoutTimer = 0;


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////// 


    private TriesUI tries;
    public GameObject triesGameObj;

    private FinalScreen finalScreen;
    public GameObject screenGameObj;


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////// 


    private void Start ()
    {
        MakeCards();
        DistributeCards();
        EndScreen();
    }

    private void Update()
    {
        RotateBackOrRemovePair();
        
        if (ScoreUI.scorePoints == 8)
        {
            finalScreen.finalScreen.enabled = true;
        }

        if (Input.GetKey(KeyCode.F))
        {

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    private void MakeCards()
    {
        CalculateAmountOfPairs();
        LoadSprites();
        SelectBackSprite();
        SelectFrontSprites();
        ConstructCards();
    }

    private void EndScreen()
    {
        finalScreen = screenGameObj.GetComponent<FinalScreen>();
        tries = triesGameObj.GetComponent<TriesUI>();
        finalScreen.finalScreen.enabled = false;
    }

    private void CalculateAmountOfPairs()
    {
        if ((columns * rows) % 2 == 0)
        {
            totalPairs = (rows * columns) * 0.5f;
        }
        else
        {
            Debug.LogError("Je hebt een oneven aantal kaarten ingevuld.");
        }
    }

    private void LoadSprites()
    {
        fronts = Resources.LoadAll<Sprite>(frontDirectory);
        backs = Resources.LoadAll<Sprite>(backDirectory);
    }

    private void SelectBackSprite()
    {
        if (backs.Length > 0)
        {
            int rnd = Random.Range(0, backs.Length);
            backSprite = backs[rnd];
        }
        else
        {
            Debug.LogError("Er zijn geen achterkant plaatjes om uit te kiezen");
        }
    }

    private void SelectFrontSprites()
    {
        if (fronts.Length < totalPairs)
        {
            Debug.LogError("Er zijn te weining plaatjes om " + totalPairs + " paren te maken.");
            return;
        }

        frontSprites = new List<Sprite>();

        while (frontSprites.Count < totalPairs)
        {
            int rnd = Random.Range(0, fronts.Length);

            if(!frontSprites.Contains(fronts[rnd]))
            {
                frontSprites.Add(fronts[rnd]);
            }
        }
    }

    private void ConstructCards()
    {
        stackOfCards = new Stack<GameObject>();

        GameObject parent = new GameObject();
        parent.name = "Cards";

        foreach (Sprite sprite in frontSprites)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject go = Instantiate(cardPrefab);
                Card cardscript = go.GetComponent<Card>();  

                cardscript.SetBack(backSprite);
                cardscript.SetFront(sprite);

                go.name = sprite.name;
                go.transform.parent = parent.transform;

                stackOfCards.Push(go);
            }
        }
    }

    private void RestartGameNoPoints()
    {
        SceneManager.LoadScene(0);
    }

    public void DistributeCards()
    {
        placedCards = new GameObject[columns, rows];
        ShuffleCards();
        PlacedCardsOnField();
    }

    private void ShuffleCards()
    {
        int[,] gridGrootte = new int[columns, rows];

        while (stackOfCards.Count > 0)
        {
            GameObject card = stackOfCards.Peek();

            int randX = Random.Range(0, columns);
            int randY = Random.Range(0, rows);

            if (placedCards[randX, randY] == null)
            {
                placedCards[randX, randY] = card;
                stackOfCards.Pop();
                //print("kaart " + card.name + " is geplaatst op x: " + randX + " y: " + randY);
            }
        }
    }

    private void PlacedCardsOnField()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                GameObject card = placedCards[x, y];

                Card cardscript = card.GetComponent<Card>();

                Vector2 cardsize = cardscript.GetBackSize();

                float x_pos = fieldAnchor.transform.position.x + (x * (cardsize.x + x_Offset));
                float y_pos = fieldAnchor.transform.position.y - (y * (cardsize.y + y_Offset));

                //print(card.transform.lossyScale.x);

                placedCards[x, y].transform.position = new Vector3(x_pos, y_pos, 0f);
            }
        }
    }

    private void CheckForMatchingPair()
    {
        timeoutTimer = 0f;

        if (selectedCards[0].name == selectedCards[1].name)
        {
            status = GameStatus.match_found;
            ScoreUI.scorePoints++;
            //Debug.Log("Score Point Added.");
        }
        else
        {
            status = GameStatus.no_match_found;
            tries.tryPoints++;
            //Debug.Log("Try Point Added.");
        }
    }

    private void RotateBackOrRemovePair()
    {
        if (status == GameStatus.match_found || status == GameStatus.no_match_found)
        {
            timeoutTimer += Time.deltaTime;

            if (timeoutTimer >= timeoutTargetTime)
            {
                if (status == GameStatus.match_found)
                {
                    selectedCards[0].SetActive(false);
                    selectedCards[1].SetActive(false);
                }
                else if (status == GameStatus.no_match_found)
                {
                    selectedCards[0].GetComponent<Card>().TurnToBack();
                    selectedCards[1].GetComponent<Card>().TurnToBack();
                }
                selectedCards[0] = null;
                selectedCards[1] = null;

                status = GameStatus.waiting_on_first_card;
            }
        }
    }

    public void SelectCard(GameObject card)
    {
        if (status == GameStatus.waiting_on_first_card)
        {
            selectedCards[0] = card;
            status = GameStatus.waiting_on_second_card;
        }
        else if (status == GameStatus.waiting_on_second_card)
        {
            selectedCards[1] = card;
            CheckForMatchingPair();
        }
    }

    public bool AllowToSelectCard(Card card)
    {
        if (selectedCards[0] == null)
        {
            return true;
        }
        if (selectedCards[1] == null)
        {
            if(selectedCards[0] != card.gameObject)
            {
                return true;
            }
        }
        return false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
        ScoreUI.scorePoints = 0;
        tries.tryPoints = 0;
    }
}
