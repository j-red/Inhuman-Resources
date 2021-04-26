using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour {
    // private Rigidbody2D rb;
    private Rigidbody rb;
    [HeaderAttribute ("Movement"), Range(1f, 10f)]
    public float speed = 5f;
    private float weight;
    private Vector3 init;

    [SerializeField, Range(0.01f, 3f)]
    private float killDelay = 2f;
    private float initDelay = 0.1f;
    private GameManager gm; // Game Manager

    private float grav = 0f;

    public enum AgentMoveType { Velocity, Position, Thrust, Torque };
    public AgentMoveType agentMoveType;

    [SerializeField]
    private bool debugMode = false;

    [HeaderAttribute ("Sound Effects")] 
    public GameObject deathPFX;
    private AudioSource audioSrc;
    public AudioClip deathSound, bumpSound;

    private float triggerTimeout = 0.3f, triggerCall = 0f;

    // Start is called before the first frame update
    void Start() {
        init = transform.position;
        // rb = GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody>();
        weight = rb.mass;
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        audioSrc = GetComponent<AudioSource>();
        gm.UpdateAgentCount();
    }

    // Update is called once per frame
    void Update() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float movement = x * speed;
        
        switch (agentMoveType) { // RB2D alternatives commented out
            case AgentMoveType.Velocity:
                // rb.velocity = new Vector2(movement, rb.velocity.y - grav); // A: update velocity directly -- works well for horizontal movement, but not for airborne/vertical
                rb.velocity = new Vector3(movement, rb.velocity.y - grav, 0);
                break;
            case AgentMoveType.Position:
                // Vector2 velocity = new Vector2(x * speed * Time.fixedDeltaTime, 0); // B: use Rigidbody2D.MovePosition -- not smooth
                Vector3 velocity = new Vector3(x * speed * Time.fixedDeltaTime, 0, 0);
                rb.MovePosition(rb.position + velocity); 
                break;
            case AgentMoveType.Thrust: // C: Add force
                // Vector2 thrust = new Vector2(movement, -grav);
                // rb.AddForce(thrust * Time.deltaTime * speed, ForceMode2D.Impulse);
                Vector3 thrust = new Vector3(movement, -grav, 0);
                rb.AddForce(thrust * Time.deltaTime * speed, ForceMode.Impulse);
                // rb.AddTorque(-x);
                rb.AddTorque(new Vector3(-x, 0, 0));
                break;
            case AgentMoveType.Torque: // D: Add rotation torque based on Horizontal axis input. See https://docs.unity3d.com/ScriptReference/Rigidbody2D.AddTorque.html.
                // rb.AddTorque(-movement);
                rb.AddTorque(new Vector3(-movement, 0, 0));
                break;
        }

        triggerCall = Mathf.Max(triggerCall - Time.deltaTime, 0f); // update time delay for trigger calls

        if (debugMode && Input.GetButtonDown("Debug Reset")) {
            transform.position = init;
            rb.velocity = Vector2.zero;
        }
    }

    void FixedUpdate() {
        /* Physics loop -- runs 50x per second. */
    }

    // private void OnTriggerEnter2D(Collider2D other) {
    //     /* Destroy Agent if they come into contact with a 'Kill Zone' */
    //     if (other.gameObject.tag == "Kill") {
    //         StartCoroutine("Kill");
    //     }
    //  
    //     if (other.gameObject.tag == "Goal" && triggerCall == 0f) {
    //         triggerCall = triggerTimeout;
    //         StartCoroutine("Goal");
    //         gm.numSaved += 1;
    //     }
    // }

    private void OnTriggerEnter(Collider other) {
        /* Destroy Agent if they come into contact with a 'Kill Zone' */
        if (other.gameObject.tag == "Kill" && triggerCall == 0f) {
            StartCoroutine("Kill");
            triggerCall = triggerTimeout;
            gm.numDead += 1;
        }
        
        if (other.gameObject.tag == "Goal" && triggerCall == 0f) {
            triggerCall = triggerTimeout;
            StartCoroutine("Goal");
            gm.numSaved += 1;
        }
    }

    private void OnCollisionEnter(Collision other) {
        audioSrc.clip = bumpSound;
        audioSrc.pitch = Random.Range(0.8f, 2f);
        audioSrc.Play();
    }

    IEnumerator Kill() {
        /* Coroutine code based on Unity manual, https://docs.unity3d.com/Manual/Coroutines.html. */

        Instantiate(deathPFX, transform.position, Quaternion.identity);
        audioSrc.pitch = Random.Range(0.5f, 1.2f);
        audioSrc.clip = deathSound;
        audioSrc.Play();

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

    IEnumerator Goal() {
        /* Coroutine code based on Unity manual, https://docs.unity3d.com/Manual/Coroutines.html. */
        Instantiate(deathPFX, transform.position, Quaternion.identity);
        // deathSound.pitch = Random.Range(0.5f, 1.2f);
        // deathSound.Play();

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
