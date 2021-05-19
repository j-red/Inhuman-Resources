using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickButton : MonoBehaviour {
    public float pushDepth = 0.075f;
    private float startHeight;
    public bool active = false;
    private Animator anim;

    [HeaderAttribute("Debug")]
    public GameObject instantiationTarget;
    public Vector3 instantiationPosition;
    private float timer = 1f;
    public bool isEnabled = true;

    private void OnMouseDown() {
        if (isEnabled)
            active = true;
    }

    private void OnMouseUp() {
        if (isEnabled)
            active = false;
    }

    // Start is called before the first frame update
    void Start() {
        startHeight = transform.position.y;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        anim.SetBool("Active", active);

        if (active && timer <= 0f) {
            Instantiate(instantiationTarget, instantiationPosition, Quaternion.identity, null);
            timer = 1f;
        }

        timer -= Time.deltaTime;
    }
}
