using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsButton : MonoBehaviour {
    public bool isActive = false;
    
    [HeaderAttribute ("Setup")] 
    public int numToActivate = 1;
    private GameObject top;
    
    [SerializeField]
    private int currentCount = 0;

    private Collider col;
    private Animator anim;

    [SerializeField]
    private float dampTime = 0.3f;
    [SerializeField]
    private float dy = -0.04f;
    private Vector3 startPos, activePos;
    
    [SerializeField]
    private float weight = 0f;

    public Animator animTarget;
    public string targetName = "Activated"; /* Boolean parameter value on animTarget animator to set to true/false */

    // Start is called before the first frame update
    void Start() {
        top = transform.Find("Top").gameObject; // reference top part of button
        col = GetComponent<Collider>();
        startPos = top.transform.position;
        activePos = startPos + new Vector3(0, dy, 0);
        // anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        weight = Mathf.SmoothStep(weight, currentCount / (float)numToActivate, dampTime);
        top.transform.position = Vector3.Slerp(startPos, activePos, weight);
        // anim.SetFloat("Weight", weight, dampTime, deltaTime);
        
        isActive = weight >= 1f; // set isActive to true when the weight exceeds the required amount

        animTarget.SetBool(targetName, isActive);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            currentCount += 1;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            currentCount -= 1;
        }
    }
}
