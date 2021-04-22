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
    private float initDelay = 0.1f;
    private GameManager gm; // Game Manager

    public float grav = 0f;

    [SerializeField]
    private bool debugMode = false;

    // Start is called before the first frame update
    void Start() {
        init = transform.position;
        rb = GetComponent<Rigidbody2D>();
        weight = rb.mass;
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float movement = x * speed;
        
        // rb.velocity = new Vector2(movement, rb.velocity.y - grav); // A: update velocity directly -- works well for horizontal movement, but not for airborne/vertical
        
        // Vector2 velocity = new Vector2(x * speed * Time.fixedDeltaTime, 0); // B: use Rigidbody2D.MovePosition -- not smooth
        // rb.MovePosition(rb.position + velocity); 

        Vector2 thrust = new Vector2(movement, -grav); // C: 
        rb.AddForce(thrust * Time.deltaTime * speed, ForceMode2D.Impulse);

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
            StartCoroutine("Kill");
        }
        
        // Debug.Log(other.gameObject.name + " : " + gameObject.name + " : " + Time.time);
    }

    IEnumerator Kill() {
        /* Coroutine code based on Unity manual, https://docs.unity3d.com/Manual/Coroutines.html. */

        for (float i = 0; i < initDelay; i += Time.deltaTime) {
            /* This code simply waits initDelay seconds before triggering the shrink effect. */
            yield return null;
        }
        
        for (float i = 0; i <= killDelay; i += Time.deltaTime) {
            transform.localScale = transform.localScale * .95f; // shrink agent by 5% each frame
            yield return null;
        }
        transform.parent = null;
        gm.UpdateAgentCount();

        Destroy(this.gameObject); /* Destroy agent GameObject after initDelay + killDelay seconds */
    }

}
