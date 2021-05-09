using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsLeverActivator : MonoBehaviour {
    private PhysicLever lev;
    public bool positive = true;

    // Start is called before the first frame update
    void Start() {
        lev = transform.parent.parent.gameObject.GetComponent<PhysicLever>();
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnTriggerEnter(Collider other) {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb != null) {
            if (positive) {
                lev.tween += rb.mass;
            } else {
                lev.tween -= rb.mass;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb != null) {
            if (positive) {
                lev.tween -= rb.mass;
            } else {
                lev.tween += rb.mass;
            }
        }
    }

} // end class
