using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [HeaderAttribute ("Debug")] 
    public bool debugMode = false;
    [Range(1, 120)]
    public int frameDelay = 15; // num frames to wait between each instance of new agent
    private int delay = 0;
    [SerializeField, Range(0, 1f)]
    private float randomOffset = 0.1f;
    private GameObject agentContainer;
    private Text agentCounter;
    private Camera cam;
    private Vector3 camPos;
    public bool isPaused = false;
    public GameObject pauseMenu;
    private Vector3 mouseStart, mouseDelta;
    private PopulateAgentDisplay agentDisp;
    private AudioSource bgAudio;
    public bool hasWon = false, hasLost = false;
    public GameObject confetti;

    private float pausedPitch = -1f; // pitch speed to lerp to when paused

    [HeaderAttribute ("Camera Controls")] 
    // [Range(30f, 120f)]
    // public float targetFOV = 40f;
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

    [HeaderAttribute ("Camera Bounds")] 
    public Vector2 lowerLeft = new Vector2(-5, -5);
    public Vector2 upperRight = new Vector2(5, 5);
    [SerializeField]
    private Vector2 currentCamPos; // useful in debugging/assigning values

    [HeaderAttribute ("Agents")] 
    public GameObject agent;
    public int numAgents = 0, numDead = 0, numSaved = 0, maxCt = 100, numToWin = 10;

    // [HeaderAttribute ("Dialogs")] 
    private DialogueTrigger dialogTrigger;

    // Start is called before the first frame update, Awake is called even earlier.
    void Awake() {
        cam = Camera.main;
        camPos = cam.transform.position;
        targetFOV = cam.fieldOfView;
        targetSize = cam.orthographicSize;

        agentContainer = GameObject.Find("Agent Container");
        agentCounter = GameObject.Find("Agent Counter").GetComponent<Text>();
        agentDisp = GameObject.Find("Agent Display").GetComponent<PopulateAgentDisplay>();
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
    }

    // Update is called once per frame
    void Update() {
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
        }

        if (numAgents == 0 && !hasWon && !hasLost) {
            Lose();
        }
    }

    // public void TriggerDialogue() {
    //     FindObjectOfType<DialogueManager>().StartDialogue(sdialogue);
    // }

    public void UpdateAgentCount() {
        // int maxCt = 36; // temp
        string formatString = "D3";
        if (!agentContainer) agentContainer = GameObject.Find("Agent Container");
        numAgents = agentContainer.transform.childCount;
        // agentCounter.text = "Agents: " + numAgents.ToString(formatString) + "/" + maxCt.ToString(formatString); // convert number of agents to string with leading zeroes

        agentCounter.text = "Agents: " + numSaved.ToString(formatString) + "/" + numToWin.ToString(formatString); // convert number of agents to string with leading zeroes

        // agentDisp.UpdateAgentColors(numAgents, maxCt - numAgents - 2, 1); // num active, inactive, dead
        agentDisp.UpdateAgentColors(numAgents, numSaved); // num active
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
            float minFOV = 30f, maxFOV = 120f;
            targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
            cam.fieldOfView = Mathf.SmoothStep(cam.fieldOfView, targetFOV, zoomSpeed);

            // Orthographic (2D) Zoom
            float minScale = 1f, maxScale = 12f;
            targetSize = Mathf.Clamp(targetSize, minScale, maxScale);
            cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, targetSize, zoomSpeed);
        }
    }

    void PauseGame() {
        Time.timeScale = 0f;
        float pauseGrain = 1f;
        if (postProcessor.profile.TryGet<FilmGrain>(out var grain)) {
            grain.intensity.value = pauseGrain;
        }

    }

    void ResumeGame() {
        Time.timeScale = 1f;

        if (postProcessor.profile.TryGet<FilmGrain>(out var grain)) {
            grain.intensity.value = initGrain;
        }
    }

    public void Win() {
        print("You won!");
        hasWon = true;

        dialogTrigger.Win();

        Instantiate(confetti, GameObject.Find("Goal Zone").transform.position, Quaternion.identity);
        return;
    }

    public void Lose() {
        hasLost = true;
        dialogTrigger.Lose();
    }

    public void ResetActiveScene() { // From https://forum.unity.com/threads/how-to-fully-reset-a-scene-upon-restart.452463/
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ResumeGame();
    }
}
