using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentPipe : MonoBehaviour {
    // public Collider ingress;
    public GameObject partner;
    public Vector3 egress;
    // [Range(1, 100f)]
    [Range(0, 25)]
    public float thrust = 5f;
    
    public bool debug = false;

    // Start is called before the first frame update
    void Start() {
        egress = partner.transform.Find("Egress").transform.position;
    }

    // Update is called once per frame
    void Update() {

    }

    private void FixedUpdate() {
        if (debug) {
            Debug.DrawLine(transform.position, transform.position + (transform.forward * thrust), Color.red);
        }
    }
}
