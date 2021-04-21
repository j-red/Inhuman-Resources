using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour {
    public bool debugMode = false;
    private Rigidbody2D rb;

    [Range(1f, 10f)]
    public float speed = 5f;
    public const float weight = 10f;

    private Vector3 init;

    // Start is called before the first frame update
    void Start() {
        init = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float movement = x * speed;
        rb.velocity = new Vector2(movement, rb.velocity.y);


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
            Destroy(this.gameObject);
        }
    }
}
