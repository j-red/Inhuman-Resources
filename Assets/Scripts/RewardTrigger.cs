using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardTrigger : MonoBehaviour {
    private GameManager gm;
    private GameObject mainCanvas;
    private float time;
    private static float starDelayTime = 1f;

    // public GameObject oneStarText, twoStarText, threeStarText;
    public float twoStarTime, threeStarTime;
    public GameObject stars, continueText;
    private int numStars = 1;

    void Awake() {
        mainCanvas = GameObject.Find("Canvas");
        if (continueText == null) continueText = GameObject.Find("Continue Text");
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if (stars == null) {
            stars = GameObject.Find("Stars");
        }
    }

    public void Win() {
        stars.SetActive(true);
        
        time = gm.gameTime;

        if (time <= threeStarTime){
            numStars = 3;
        } else if (time <= twoStarTime){
            numStars = 2;
        }

        StartCoroutine("ShowStars");

        return;
    }

    IEnumerator ShowStars() {
        for (int i = 0; i < 3; i ++) {
            print("Setting index " + i.ToString());
            GameObject currentStar = stars.transform.GetChild(i).gameObject;
            currentStar.GetComponent<SpriteRandomizer>().useAlts = i >= numStars; // if current star is good, fill it in. else, use hollow sprite
            currentStar.GetComponent<Animator>().SetTrigger("Spin");
            yield return new WaitForSeconds(starDelayTime);
        }

        yield return new WaitForSeconds(1f);
        continueText.gameObject.SetActive(true);
    }
}