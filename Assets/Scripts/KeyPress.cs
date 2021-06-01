using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPress : MonoBehaviour {
    /* These settings control what keypresses this instance of the script will listen for. */
    public bool agentDisplayToggle = false;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (agentDisplayToggle) {
            if (Input.GetButtonDown("Toggle Menu")) {
                GameObject.Find("Canvas").GetComponent<AnimationManager>().ToggleBool("On");
            }
        } // end if agent display toggle

    } // end update
} // end monobehaviour
