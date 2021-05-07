using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
    private GameManager gm;
    private GameObject mainCanvas;

    public GameObject introText, loseText, winText;
    
    // Start is called before the first frame update
    void Awake() {
        mainCanvas = GameObject.Find("Canvas");
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void Intro() {
        gm.DisableAllDialogues();
        if (introText != null) {
            Instantiate(introText, mainCanvas.transform);
        }
        // new.GetComponent<Transform>().SetSiblingIndex(0);
    }

    public void Win() {
        gm.DisableAllDialogues();
        if (winText != null) {
            Instantiate(winText, mainCanvas.transform);
        }
        // new.GetComponent<Transform>().SetSiblingIndex(0);
    }

    public void Lose() {
        gm.DisableAllDialogues();
        if (loseText != null) {
            Instantiate(loseText, mainCanvas.transform);
        }
        // new.GetComponent<Transform>().SetSiblingIndex(0);
    }
}