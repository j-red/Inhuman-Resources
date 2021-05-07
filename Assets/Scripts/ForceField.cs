using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour {
    [Range(0, 100f)]
    public float thrust = 25f;
    [SerializeField]
    private float scalar = 1000f;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnTriggerStay(Collider other) {
        Rigidbody body = other.gameObject.GetComponent<Rigidbody>();
        if (body != null) {
            body.AddForce(transform.up * thrust * scalar);
        }
    }
}
