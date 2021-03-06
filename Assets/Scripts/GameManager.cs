using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [HeaderAttribute ("Core Logic")] 
    private GameObject agentContainer;
    private Text agentCounter;
    private Camera cam;
    private Vector3 camPos;
    public bool isPaused = false;
    public GameObject pauseMenu;
    private Vector3 mouseStart, mouseDelta;
    // private PopulateAgentDisplay agentDisp;
    private AudioSource bgAudio;
    public bool hasWon = false;
    public bool hasLost = false;
    public GameObject confetti;
    public GameObject FadeToBlack;

    private float pausedPitch = -1f; // pitch speed to lerp to when paused

    [HeaderAttribute ("Camera Controls")] 
    private float targetSize, targetFOV;
    [SerializeField, Range(0, 10f)]
    private float zoomScalar = 3f;
    [SerializeField, Range(0, 0.5f)]
    private float dragScale = 0.02f;
    [SerializeField, Range(0, 10f)]
    private float zoomSpeed = 5f;    
    private Volume postProcessor;
    private float audioSwitchSpeed = 0.1f;
    private float initGrain;
    private float startDelay = 5f; // wait 5 seconds before allowing lose conditions to be met with 0 agents

    [HeaderAttribute ("Camera Bounds")] 
    public float minFOV = 30f, maxFOV = 120f;
    public Vector2 lowerLeft = new Vector2(-5, -5);
    public Vector2 upperRight = new Vector2(5, 5);
    [SerializeField]
    private Vector2 currentCamPos; // useful in debugging/assigning values

    [HeaderAttribute ("Agents")] 
    public GameObject agent;
    public int numAgents = 0, numDead = 0, numSaved = 0, maxCt = 100, numToWin = 10;
    public string counterText = "Agents Recovered: ";

    [HeaderAttribute("Setup")]
    public float gameTime = 0f;
    private Text gameTimer;

    [SerializeField, HeaderAttribute("Debug")]
    public bool debugMode = false;
    [Range(1, 120)]
    public int frameDelay = 15; // num frames to wait between each instance of new agent
    private int delay = 0;
    [SerializeField, Range(0, 1f)]
    private float randomOffset = 0.1f;
    private DialogueTrigger dialogTrigger;
    private RewardTrigger rewardTrigger;
    private float endGameTimer = 0f;
    private bool hasEnded = false;
    public GameObject EndGameDialogue;

    // Start is called before the first frame update, Awake is called even earlier.
    void Awake() {
        cam = Camera.main;
        camPos = cam.transform.position;
        targetFOV = cam.fieldOfView;
        targetSize = cam.orthographicSize;

        agentContainer = GameObject.Find("Agent Container");
        agentCounter = GameObject.Find("Agent Counter").GetComponent<Text>();
        // agentDisp = GameObject.Find("Agent Display").GetComponent<PopulateAgentDisplay>();
        bgAudio = GameObject.Find("Background Music").GetComponent<AudioSource>();
        postProcessor = GameObject.Find("Post Processing Volume").GetComponent<Volume>();

        if (postProcessor.profile.TryGet<FilmGrain>(out var grain)) {
            initGrain = grain.intensity.value;
        }

        UpdateAgentCount();
    }

    private void Start() {
        dialogTrigger = GetComponent<DialogueTrigger>();
        dialogTrigger.Intro();

        rewardTrigger = GetComponent<RewardTrigger>();
        gameTimer = GameObject.Find("Game Timer").GetComponent<Text>();
    }

    public bool stopTime = false;
    public void StopTimer() {
        stopTime = true;
    }

    // Update is called once per frame
    void Update() {
        if ((!isPaused && !hasWon && !stopTime)) {
            gameTime += Time.deltaTime; // increment game time clock whenever unpaused
            string fmat = "D2"; // use 2 leading zeros in timer text

            int ms = (int)((gameTime % 1) * 1000),
                min = (int)(gameTime / 60),
                s = (int)(gameTime % 60);
            gameTimer.text = "Time: " + min.ToString(fmat) + ":" + s.ToString(fmat) + ":" + ms.ToString("D3");
        }
            

        if (debugMode && Input.GetMouseButton(1) && !isPaused) {
            /* If debug mode is enabled and right mouse button pressed, create new agents at mouse position: */
            delay -= 1;
            if (delay <= 0) {
                Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                newPos.x += Random.Range(-randomOffset, randomOffset);
                newPos.y += Random.Range(-randomOffset, randomOffset);
                newPos.z = 0f;
                GameObject clone = Instantiate(agent, newPos, Quaternion.identity);
                clone.transform.parent = agentContainer.transform;
                delay = frameDelay;

                // UpdateAgentCount();
            }
        }

        if (Input.GetButtonDown("Pause")) {
            if (isPaused) {
                ResumeGame();    
            } else {
                PauseGame();
            }
            isPaused = !isPaused;
            pauseMenu.SetActive(isPaused);
            pauseMenu.transform.SetAsLastSibling();
        }

        if (isPaused) { // Smooth switch from playing in reverse to playing forward for background music when paused.   
            bgAudio.pitch = Mathf.SmoothStep(bgAudio.pitch, pausedPitch, audioSwitchSpeed);
        } else {
            bgAudio.pitch = Mathf.SmoothStep(bgAudio.pitch, 1f, audioSwitchSpeed);
        }

        if (numSaved >= numToWin && !hasWon) {
            Win();
        } else if (numAgents <= 0 && !hasWon && !hasLost && gameTime >= startDelay) {
            Lose();
        }


        /* End current level logic */
        if (hasWon && Input.GetButtonDown("Submit") && GameObject.Find("FadeToBlack") == null) {
            MoveToNextLevel();
            Destroy(GameObject.Find("Agent Container"), 5); // remove one agent per frame after winning

            endGameTimer = 5f;
        }

        if (hasWon && !hasEnded && endGameTimer > 0) { /* Timer to delay instantiation of endgame dialogue. */
            endGameTimer -= Time.deltaTime;

            if (endGameTimer <= 0) {
                Instantiate(EndGameDialogue, GameObject.Find("Canvas").transform);
                hasEnded = true;
            }
        }
    }

    public void UpdateAgentCount() {
        // int maxCt = 36; // temp
        string formatString = "D3";
        if (!agentContainer) agentContainer = GameObject.Find("Agent Container");
        numAgents = agentContainer.transform.childCount;
        // agentCounter.text = "Agents: " + numAgents.ToString(formatString) + "/" + maxCt.ToString(formatString); // convert number of agents to string with leading zeroes

        agentCounter.text = counterText + numSaved.ToString(formatString) + "/" + numToWin.ToString(formatString); // convert number of agents to string with leading zeroes

        // agentDisp.UpdateAgentColors(numAgents, maxCt - numAgents - 2, 1); // num active, inactive, dead
        // agentDisp.UpdateAgentColors(numAgents, numSaved); // this used to add icons for each of the agents
    }

    void LateUpdate() {
        /* Mouse Zoom and Camera Drag */
        if (!isPaused) {
            targetSize -= Input.GetAxis("Mouse ScrollWheel") * zoomScalar;
            targetFOV -= Input.GetAxis("Mouse ScrollWheel") * zoomScalar * 10f;

            if (Input.GetButtonDown("Pan")) { /* When Middle Mouse is pressed: */
                mouseStart = Input.mousePosition;
                camPos = cam.transform.position;
            } else if (Input.GetButton("Pan")) { // Middle mouse held down:
                mouseDelta = Input.mousePosition - mouseStart;
                cam.transform.position = camPos - mouseDelta * dragScale;
            }

            // Clamp camera position to bounds indicated by LowerLeft and UpperRight.
            cam.transform.position = new Vector3(Mathf.Clamp(cam.transform.position.x, lowerLeft.x, upperRight.x), Mathf.Clamp(cam.transform.position.y, lowerLeft.y, upperRight.y), cam.transform.position.z);
            currentCamPos = new Vector2(cam.transform.position.x, cam.transform.position.y);

            // Perspective (3D) Zoom
            targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
            cam.fieldOfView = Mathf.SmoothStep(cam.fieldOfView, targetFOV, zoomSpeed);

            // Orthographic (2D) Zoom
            float minScale = 1f, maxScale = 12f;
            targetSize = Mathf.Clamp(targetSize, minScale, maxScale);
            cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, targetSize, zoomSpeed);
        }
    }

    public void PauseGame() {
        Time.timeScale = 0f;
        float pauseGrain = 1f;
        if (postProcessor.profile.TryGet<FilmGrain>(out var grain)) {
            grain.intensity.value = pauseGrain;
        }

    }

    public void ResumeGame() {
        Time.timeScale = 1f;

        if (postProcessor.profile.TryGet<FilmGrain>(out var grain)) {
            grain.intensity.value = initGrain;
        }
    }

    public void Win() {
        hasWon = true;

        dialogTrigger.Win();
        rewardTrigger.Win();

        Instantiate(confetti, GameObject.Find("Goal Zone").transform.position, Quaternion.identity);
        return;
    }

    public void Lose() {
        hasLost = true;
        dialogTrigger.Lose();
    }

    public void ResetActiveScene() { // From https://forum.unity.com/threads/how-to-fully-reset-a-scene-upon-restart.452463/
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        DisableAllDialogues();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ResumeGame();
        hasLost = false;
        hasWon = false;
    }

    public void DisableAllDialogues() {
        GameObject[] dialogues = GameObject.FindGameObjectsWithTag("Dialogue");
        foreach (GameObject box in dialogues) {
            Destroy(box);
        }
    }

    public void MoveToNextLevel() {
        /* Eventually, this should contain the logic for moving to the next scene.
           For now, this simply instantiates a 'Fade to Black' UI Prefab. */
        Instantiate(FadeToBlack, GameObject.Find("Canvas").transform);
        StartCoroutine("FadeAudio");
        // Invoke("LoadMainMenu", 2f);
        Invoke("LoadNextScene", 2f);
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

    public void LoadNextScene() {
        // Loads the scene after this one in the Build Settings queue.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
