using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardTrigger : MonoBehaviour
{
    private GameManager gm;
    private GameObject mainCanvas;
    private float time;

    // public GameObject oneStarText, twoStarText, threeStarText;
    public float oneStarTime, twoStarTime, threeStarTime;
    public GameObject firstStar, secondStar, thirdStar;
    public GameObject emptyStarTwo, emptyStarThree;


    void Awake() {
        mainCanvas = GameObject.Find("Canvas");
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void Win() {
        time = gm.gameTime;
        print(time <= threeStarTime);
        if (time <= threeStarTime){
            ThreeStar();
        } else if (time <= twoStarTime){
            TwoStar();
        } else {
            OneStar();
        }
        return;
    }

    void OneStar() {
        Instantiate(firstStar, mainCanvas.transform);
        Instantiate(emptyStarTwo, mainCanvas.transform);
        Instantiate(emptyStarThree, mainCanvas.transform);
        return;
    }

    void TwoStar() {
        Instantiate(firstStar, mainCanvas.transform);
        Instantiate(secondStar, mainCanvas.transform);
        Instantiate(emptyStarThree, mainCanvas.transform);
        return;
    }

    void ThreeStar() {
        Instantiate(firstStar, mainCanvas.transform);
        Instantiate(secondStar, mainCanvas.transform);
        Instantiate(thirdStar, mainCanvas.transform);
        return;
    }
}