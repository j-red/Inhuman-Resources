using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class L3Custom : MonoBehaviour {
    private GameManager gm;
    private bool hasWon = false;

    // Start is called before the first frame update
    void Start() {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update() {
        if (gm.hasLost && !hasWon) {
            GetComponent<RewardTrigger>().Win();
            hasWon = true;
            gm.StopTimer();
        }

        if (hasWon && Input.GetButtonDown("Submit")) {
            MainMenu();
        }
    }

    public void MainMenu() {
        Instantiate(gm.FadeToBlack, GameObject.Find("Canvas").transform);
        StartCoroutine("FadeAudio");
        Invoke("LoadMainMenu", 2f);
    }

    IEnumerator FadeAudio() {
        GameObject bg = GameObject.Find("Background Music");
        AudioSource bgMusic;

        if (bg != null) {
            bgMusic = bg.GetComponent<AudioSource>();
        } else {
            yield break;
        }

        float initVol = bgMusic.volume;

        for (float i = 0; i < 2f; i += Time.deltaTime) {    
            bgMusic.volume = Mathf.Lerp(initVol, 0, i);
            yield return null;
        }
    }

    public void LoadMainMenu() {
        /* usage: Invoke("LoadMainMenu", 5); // calls this function in 5 seconds */
        SceneManager.LoadScene("Menu");
    }
}
