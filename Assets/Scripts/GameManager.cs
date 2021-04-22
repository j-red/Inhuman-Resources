using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public bool debugMode = false;
    [Range(1, 120)]
    public int frameDelay = 12; // num frames to wait between each instance of new agent
    private int delay = 0;
    [SerializeField, Range(0, 1f)]
    private float randomOffset = 0.1f;
    public GameObject agent;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (debugMode && Input.GetMouseButton(0)) {
            /* If debug mode is enabled and left mouse button pressed, create new agents at mouse position: */
            delay -= 1;
            if (delay <= 0) {
                Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                newPos.x += Random.Range(-randomOffset, randomOffset);
                newPos.y += Random.Range(-randomOffset, randomOffset);
                newPos.z = 0f;
                Instantiate(agent, newPos, Quaternion.identity);
                delay = frameDelay;
            }
        }
    }
}
