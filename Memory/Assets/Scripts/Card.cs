using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {

    [SerializeField] private CardStatus status;

    private float turnTargetTime = .15f;
    private float turnTimer = 1.6f;

    [SerializeField] SpriteRenderer frontRenderer;
    [SerializeField] SpriteRenderer backRenderer;

    private Quaternion startRotation;

    public enum CardStatus
    {
        showBack = 0,
        showFront,
        rotatingToBack,
        rotatingToFront
    }

    private Game game;

    private void Awake()
    {
        game = FindObjectOfType<Game>();
        status = CardStatus.showBack;
        GetFrontAndBackSpriteRenderers();
    }
   
    void FixedUpdate ()
    {
		if (status == CardStatus.rotatingToFront || status == CardStatus.rotatingToBack)
        {
            turnTimer = turnTimer + Time.deltaTime;
            if(turnTargetTime == 0f) { Debug.LogError("turnTargetTime not set in Unity."); }
            float percentage = (1f / turnTargetTime) * turnTimer;

            //========================================================================================

            float targetRotation = 0f;
            if (status == CardStatus.rotatingToFront)
            {
                targetRotation = 180f;
            }

            transform.rotation = Quaternion.Slerp(startRotation, Quaternion.Euler(0f, targetRotation, 0f), percentage);

            if (percentage >= 1f)
            {
                if(status == CardStatus.rotatingToBack)
                {
                    status = CardStatus.showBack;
                }
                else if (status == CardStatus.rotatingToFront)
                {
                    status = CardStatus.showFront;

                    //TODO: Dit komt er later pas bij:
                    game.SelectCard(gameObject);
                }
            }
        }
    }

    private void OnMouseUp()
    {
        //DEBUG: print("OnMouseUp pressed on Card");
        
        if (game.AllowToSelectCard(this) == true)
        {
            if (status == CardStatus.showBack)
            {
                TurnToFront();
            }
            else if (status == CardStatus.showFront)
            {
                TurnToBack();
            }
        }
    }

    public void TurnToFront()
    {
        turnTimer = 0f;
        startRotation = transform.rotation;
        status = CardStatus.rotatingToFront;
    }

    public void TurnToBack()
    {
        turnTimer = 0f;
        startRotation = transform.rotation;
        status = CardStatus.rotatingToBack;
    }

    private void GetFrontAndBackSpriteRenderers()
    {
        foreach(Transform t in transform)
        {
            if (t.name == "Front")
            {
                frontRenderer = t.GetComponent<SpriteRenderer>();
            }
            else if (t.name == "Back")
            {
                backRenderer = t.GetComponent<SpriteRenderer>();
            }
        }
    }

    public void SetFront(Sprite s)
    {
        frontRenderer.sprite = s;
    }

    public void SetBack(Sprite s)
    {
        backRenderer.sprite = s;
    }

    public Vector2 GetFrontSize()
    {
        return frontRenderer.bounds.size;
    }

    public Vector2 GetBackSize()
    {
        return backRenderer.bounds.size;
    }
}
