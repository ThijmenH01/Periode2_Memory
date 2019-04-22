using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour {

    public Text scoreText;
    public static int scorePoints = 0;
	
	void Update ()
    {
        scoreText.text = scorePoints.ToString();
    }
}
