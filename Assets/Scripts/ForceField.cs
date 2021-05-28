using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour {
    [Range(0, 100f)]
    public float thrust = 25f;
    [SerializeField]
    private float scalar = 100f;
    public Vector3 thrustDirection = new Vector3(0, 1f, 0); // default is up (green Y-axis)

    public static bool debug = false;
    

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (thrustDirection.magnitude > 1f)
            thrustDirection = thrustDirection.normalized;
    }

    private void FixedUpdate() {
        if (debug) {
            float q = 1f;
            Color color = new Color(q, q, 1.0f);
            // Debug.DrawLine(transform.position, transform.position + thrustDirection, color);
            Debug.DrawLine(transform.position, transform.position + (transform.forward * thrust / 2f), color);
        }
    }

    private void OnTriggerStay(Collider other) {
        Rigidbody body = other.gameObject.GetComponent<Rigidbody>();
        if (body != null) {
            body.AddForce(thrustDirection * thrust * scalar);
        }
    }
}
