using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalScreen : MonoBehaviour {

    public Text finalScreen;
    private TriesUI tries;

    public GameObject triesGameObj;

    private void Start()
    {
        tries = triesGameObj.GetComponent<TriesUI>();
    }

    void Update ()
    {
        finalScreen.text = "Misses: " + tries.tryPoints.ToString() + "\n\n\n\n" +
            "Hit R to play again.";
	}
}
