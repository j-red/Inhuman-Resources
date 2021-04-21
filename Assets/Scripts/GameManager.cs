using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public bool debugMode = false;

    public GameObject agent;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (debugMode && Input.GetMouseButton(0)) {
            /* If debug mode is enabled and left mouse button pressed: */
            Instantiate(agent, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}
