using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour {
    private Animator anim;

    // Start is called before the first frame update
    void Start() {
        anim = GetComponent<Animator>();    
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void SetBool(string name, bool value) {
        anim.SetBool(name, value);
    }

    public void ToggleBool(string name) {
        anim.SetBool(name, !anim.GetBool(name));
    }
}
