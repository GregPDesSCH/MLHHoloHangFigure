using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterListener : MonoBehaviour
{
    public int letterIndex = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnMouseUp()
    {
        GameManager.gameManager.checkLetterSelected(letterIndex);
        gameObject.GetComponent<Button>().interactable = false;
    }
}
