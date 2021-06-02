using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour {
    public GameObject fadeToBlack;
    private string toLoad;
    private static float loadDelay = 2f; // wait this many seconds before actually loading into new level

    public void LoadLevel(string levelName) {
        toLoad = levelName;

        if (fadeToBlack != null) {
            Instantiate(fadeToBlack, GameObject.Find("Canvas").transform);
            StartCoroutine("FadeAudio");
            Invoke("LoadActive", loadDelay);
        } else {
            LoadActive();
        }
        
        // SceneManager.LoadScene(levelName);
        // GameObject g = GameObject.Find("Game Manager");
        // if (g != null) {
        //     GameManager gm = g.GetComponent<GameManager>();
        //     gm.ResumeGame(); // unpause game
        // }
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

        for (float i = 0; i < loadDelay; i += Time.deltaTime) {    
            bgMusic.volume = Mathf.Lerp(initVol, 0, i);
            yield return null;
        }
    }

    private void LoadActive() {
        /* Wrapper to be able to delay actually loading into new level. */
        SceneManager.LoadScene(toLoad);
        GameObject g = GameObject.Find("Game Manager");
        if (g != null) {
            GameManager gm = g.GetComponent<GameManager>();
            gm.ResumeGame(); // unpause game
        }
    }
}
