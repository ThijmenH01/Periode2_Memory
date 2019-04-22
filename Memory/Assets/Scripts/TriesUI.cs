using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriesUI : MonoBehaviour {

    public Text triesText;
    public int tryPoints = 0;

	void Update ()
    {
        triesText.text = tryPoints.ToString();
    }
}
