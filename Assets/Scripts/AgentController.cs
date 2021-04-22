using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour {
    private Rigidbody2D rb;

    [Range(1f, 10f)]
    public float speed = 5f;
    private float weight;
    private Vector3 init;

    [SerializeField, Range(0.01f, 3f)]
    private float killDelay = 2f;
    [SerializeField, Range(0.01f, 1f)]
    private float initDelay = 0.1f;

    [SerializeField]
    private bool debugMode = false;

    public float grav = 1f;

    // Start is called before the first frame update
    void Start() {
        init = transform.position;
        rb = GetComponent<Rigidbody2D>();
        weight = rb.mass;
    }

    // Update is called once per frame
    void Update() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float movement = x * speed;
        rb.velocity = new Vector2(movement, rb.velocity.y - grav);


        if (debugMode && Input.GetButtonDown("Debug Reset")) {
            transform.position = init;
            rb.velocity = Vector2.zero;
        }
    }

    void FixedUpdate() {
        /* Physics loop -- runs 50x per second. */
    }

    private void OnTriggerEnter2D(Collider2D other) {
        /* Destroy Agent if they come into contact with a 'Kill Zone' */
        if (other.gameObject.tag == "Kill") {
            StartCoroutine("Shrink");
            // Destroy(this.gameObject, killDelay);
        }
    }

    IEnumerator Shrink() {
        /* Coroutine code based on Unity manual, https://docs.unity3d.com/Manual/Coroutines.html. */
        // float initDelay = 1f;

        for (float i = 0; i < initDelay; i += Time.deltaTime) {
            /* This code simply waits initDelay seconds before triggering the shrink effect. */
            yield return null;
        }
        
        for (float i = 0; i <= killDelay; i += Time.deltaTime) {
            transform.localScale = transform.localScale * .95f; // shrink agent by 5% each frame
            yield return null;
        }
        Destroy(this.gameObject);
    }

}
