using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicLever : MonoBehaviour {
    public bool isActive = false;
    
    [HeaderAttribute ("Setup")] 
    private GameObject handle;
    private Collider col;
    private Animator anim;
    
    // public float weight = 0f, currentWeight = 0f, weightToActivate = 1f;

    [HeaderAttribute ("Activation Fields"), Tooltip("Target inspector and boolean parameter name to activate when button is fully pressed.")] 
    public Animator animTarget;
    public string targetName = "Activated"; /* Boolean parameter value on animTarget animator to set to true/false */

    public IndicatorLight myLight;
    public AudioClip clickOn, clickOff;
    private AudioSource audioSrc;

    [HeaderAttribute ("Variable Fields"), Tooltip("These values used in scripts of children.")] 
    [Range(0, 10f)]
    public float threshold = 5f;
    public float tween = 0f;


    // Start is called before the first frame update
    void Start() {
        handle = transform.Find("Handle").gameObject;
        col = handle.GetComponent<Collider>();
        anim = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        isActive = tween >= threshold;
        
        if (isActive) {
            anim.SetBool("Active", true);
        }
        
        // if (currentWeight >= weightToActivate && !isActive) {
        //     /* If the button will become activated this frame: */
        //     PlaySound(clickOn);
        // } else if (currentWeight < weightToActivate && isActive) {
        //     /* If button is deactivating this frame: */
        //     PlaySound(clickOff);
        // }

        if (animTarget != null) {
            animTarget.SetBool(targetName, isActive);
        }
        
        if (myLight != null) {
            myLight.active = isActive;
        }
    }

    public void PlaySound(AudioClip clip) {
        audioSrc.clip = clip;
        audioSrc.Play();
    }
}
