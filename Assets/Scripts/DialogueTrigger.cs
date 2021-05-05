using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
    private GameManager gm;
    private GameObject mainCanvas;

    public GameObject introText, loseText, winText;
    
    // Start is called before the first frame update
    void Start() {
        mainCanvas = GameObject.Find("Canvas");
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void Intro() {
        Instantiate(introText, mainCanvas.transform);
        // new.GetComponent<Transform>().SetSiblingIndex(0);
    }

    public void Win() {
        Instantiate(winText, mainCanvas.transform);
        // new.GetComponent<Transform>().SetSiblingIndex(0);
    }

    public void Lose() {
        Instantiate(loseText, mainCanvas.transform);
        // new.GetComponent<Transform>().SetSiblingIndex(0);
    }
}
