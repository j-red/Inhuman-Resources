using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private float targetSize;
    [SerializeField, Range(0, 10f)]
    private float zoomScalar = 3f;
    [SerializeField, Range(0, 0.5f)]
    private float dragScale = 0.02f;
    [SerializeField, Range(0, 10f)]
    private float zoomSpeed = 5f;

    private Vector3 mouseStart, mouseDelta;
    private PopulateAgentDisplay agentDisp;

    // Start is called before the first frame update
    void Start() {
        cam = Camera.main;
        camPos = cam.transform.position;

        targetSize = cam.orthographicSize;

        agentContainer = GameObject.Find("Agent Container");
        agentCounter = GameObject.Find("Agent Counter").GetComponent<Text>();

        agentDisp = GameObject.Find("Agent Display").GetComponent<PopulateAgentDisplay>();

        UpdateAgentCount();
    }

    // Update is called once per frame
    void Update() {
        if (debugMode && Input.GetMouseButton(0)) {
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

        /* Mouse Zoom and Camera Drag */
        targetSize -= Input.GetAxis("Mouse ScrollWheel") * zoomScalar;
        if (Input.GetButtonDown("Tertiary")) { /* When Middle Mouse is pressed: */
            mouseStart = Input.mousePosition;
            camPos = cam.transform.position;
        } else if (Input.GetButton("Tertiary")) { // Middle mouse held down:
            mouseDelta = Input.mousePosition - mouseStart;
            cam.transform.position = camPos - mouseDelta * dragScale;
        }
        
    }

    public void UpdateAgentCount() {
        int maxCt = 100; // temp
        int numAgents = agentContainer.transform.childCount;
        agentCounter.text = "Agents: " + numAgents.ToString("D3") + "/" + maxCt.ToString(); // convert number of agents to string with leading zeroes

        // agentDisp.UpdateAgentColors(numAgents, maxCt - numAgents - 2, 1); // num active, inactive, dead
        agentDisp.UpdateAgentColors(numAgents); // num active
    }

    void LateUpdate() {
        float minScale = 1f, maxScale = 12f;
        targetSize = Mathf.Clamp(targetSize, minScale, maxScale);
        cam.orthographicSize = Mathf.SmoothStep(cam.orthographicSize, targetSize, zoomSpeed);
    }
}
