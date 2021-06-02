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
    private AudioSource audioSrc;
    public AudioClip sfx;

    // Start is called before the first frame update
    void Start() {
        audioSrc = GetComponent<AudioSource>();

        if (partner != null)
            egress = partner.transform.Find("Egress").transform.position;
    }
    
    private void FixedUpdate() {
        if (debug) {
            Debug.DrawLine(transform.position, transform.position + (transform.forward * thrust), Color.red);
        }
    }

    public void PlaySound() {
        if (sfx != null) audioSrc.PlayOneShot(sfx);
    }
}
