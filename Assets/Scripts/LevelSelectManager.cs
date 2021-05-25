using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour {
    public void LoadLevel(string levelName) {
        SceneManager.LoadScene(levelName);

        GameManager gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if (gm != null) {
            gm.ResumeGame(); // unpause game
        }
    }
}
