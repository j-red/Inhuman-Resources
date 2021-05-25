using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour {
    public void LoadLevel(string levelName) {
        SceneManager.LoadScene(levelName);

        GameObject g = GameObject.Find("Game Manager");

        if (g != null) {
            GameManager gm = g.GetComponent<GameManager>();
            gm.ResumeGame(); // unpause game
        }
    }
}
