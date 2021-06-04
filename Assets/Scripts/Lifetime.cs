using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour {
    [Range(0, 120f)]
    public float lifetime = 10f;
    public float age = 0f;
    public GameObject deathPfx;
    private bool hasDied = false;

    public string animTriggerName = "";
    
    // Update is called once per frame
    void Update() {
        age += Time.deltaTime;
        if (age >= lifetime) {
            if (deathPfx != null && !hasDied) Instantiate(deathPfx, transform.position, Quaternion.identity, null);
            if (animTriggerName != "") {
                GetComponent<Animator>().SetTrigger(animTriggerName);
            }
            hasDied = true;
            
            transform.localScale = transform.localScale * 0.9f;
            Destroy(this.gameObject, 1f);
        }
    }
}
