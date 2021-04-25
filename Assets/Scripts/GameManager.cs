using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour {
    public bool debugMode = false;
    [Range(1, 120)]
    public int frameDelay = 15; // num frames to wait between each instance of new agent
    private int delay = 0;
    [SerializeField, Range(0, 1f)]
    private float randomOffset = 0.1f;
    public GameObject agent;
    private GameObject agentContainer;
    private Text agentCounter;
    private Camera cam;
    private Vector3 camPos;
    private float targetSize, targetFOV;
    [SerializeField, Range(0, 10f)]
    private float zoomScalar = 3f;
    [SerializeField, Range(0, 0.5f)]
    private float dragScale = 0.02f;
    [SerializeField, Range(0, 10f)]
    private float zoomSpeed = 5f;
    private Vector3 mouseStart, mouseDelta;
    private PopulateAgentDisplay agentDisp;
    public bool isPaused = false;
    private AudioSource bgAudio;
    public GameObject pauseMenu;
    private Volume postProcessor;
    private float audioSwitchSpeed = 0.1f;
    private float initGrain;

    public int numAgents = 0, numDead = 0, numSaved = 0, maxCt = 100;

    // Start is called before the first frame update
    void Start() {
        cam = Camera.main;
        camPos = cam.transform.position;

        targetSize = cam.orthographicSize;

        agentContainer = GameObject.Find("Agent Container");
        agentCounter = GameObject.Find("Agent Counter").GetComponent<Text>();
        agentDisp = GameObject.Find("Agent Display").GetComponent<PopulateAgentDisplay>();
        bgAudio = GameObject.Find("Background Music").GetComponent<AudioSource>();
        postProcessor = GameObject.Find("Post Processing Volume").GetComponent<Volume>();
        // pauseMenu = GameObject.Find("Pause Menu");
        // pauseMenu.SetActive(false);

        // if (postProcessor.profile.TryGet<Bloom>(out var bloom)) {
        //     // bloom.intensity.overrideState = true;
        //     defaultBloom = bloom.intensity.value;
        // }

        if (postProcessor.profile.TryGet<FilmGrain>(out var grain)) {
            initGrain = grain.intensity.value;
        }

        UpdateAgentCount();
    }

    // Update is called once per frame
    void Update() {
        if (debugMode && Input.GetMouseButton(0) && !isPaused) {
            /* If debug mode is enabled and left mouse button pressed, create new agents at mouse position: */
            delay -= 1;
            if (delay <= 0) {
                Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                newPos.x += Random.Range(-randomOffset, randomOffset);
                newPos.y += Random.Range(-randomOffset, randomOffset);
                newPos.z = 0f;
                GameObject clone = Instantiate(agent, newPos, Quaternion.identity);
                clone.transform.parent = agentContainer.transform;
                delay = frameDelay;

                UpdateAgentCount();
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
        }

        if (isPaused) { // Smooth switch from playing in reverse to playing forward for background music when paused.   
            bgAudio.pitch = Mathf.SmoothStep(bgAudio.pitch, -1f, audioSwitchSpeed);
        } else {
            bgAudio.pitch = Mathf.SmoothStep(bgAudio.pitch, 1f, audioSwitchSpeed);
        }


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
        }   
    }

    public void UpdateAgentCount() {
        int maxCt = 36; // temp
        string formatString = "D3";
        numAgents = agentContainer.transform.childCount;
        agentCounter.text = "Agents: " + numAgents.ToString(formatString) + "/" + maxCt.ToString(formatString); // convert number of agents to string with leading zeroes

        // agentDisp.UpdateAgentColors(numAgents, maxCt - numAgents - 2, 1); // num active, inactive, dead
        agentDisp.UpdateAgentColors(numAgents, numSaved); // num active
    }

    void LateUpdate() {
        if (!isPaused) {
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

    void Win() {
        return;
    }
}
