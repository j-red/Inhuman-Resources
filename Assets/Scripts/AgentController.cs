using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour {
    // private Rigidbody2D rb;
    private Rigidbody rb;
    [HeaderAttribute ("Movement"), Range(1f, 10f)]
    public float speed = 5f;
    public float maxSpeed = 10f;
    private float weight;
    private Vector3 init;

    [SerializeField, Range(0.01f, 3f)]
    private float killDelay = 2f;
    private float initDelay = 0.1f;
    private static GameManager gm = null; // Game Manager

    private float grav = 0f;

    public enum AgentMoveType { Velocity, Position, Thrust, Torque, ThrustCapped };
    public AgentMoveType agentMoveType;
    public float airControl = 0.1f;

    private bool onGround = false;

    public ForceMode mode;

    [HeaderAttribute ("Sound Effects")] 
    public GameObject deathPFX;
    private AudioSource audioSrc;
    public AudioClip deathSound, bumpSound, rescueSound;

    [HeaderAttribute ("Debug"), Range(1f, 10f), SerializeField]
    private bool debugMode = false;
    public bool invincible = false;
    public bool noControl = false;
    private float triggerTimeout = 0.3f, triggerCall = 0f;

    [HeaderAttribute ("Camera Shake")] 
    public bool cameraShake = true;
    private CameraShake cs;
    public float shakeAmount = 0.1f, shakeDuration = 0.02f;

    // Start is called before the first frame update
    void Start() {
        init = transform.position;
        // rb = GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody>();
        weight = rb.mass;
        GameObject g = GameObject.Find("Game Manager");
        if (g != null) {
            gm = g.GetComponent<GameManager>();
            gm.UpdateAgentCount();
        }
        audioSrc = GetComponent<AudioSource>();
        cs = Camera.main.gameObject.GetComponent<CameraShake>();
    }

    // Update is called once per frame
    void Update() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float movement = x * speed;
        
        if (!noControl) {
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
                    if (onGround) {
                        Vector3 motion = new Vector3(movement, -grav, 0);
                        rb.AddForce(motion * Time.deltaTime * speed, mode);
                        rb.AddTorque(new Vector3(-x, 0, 0));
                    } else {
                        Vector3 motion = new Vector3(movement, -grav, 0);
                        rb.AddForce(motion * Time.deltaTime * speed * airControl, mode);
                        rb.AddTorque(new Vector3(-x, 0, 0));
                    }
                    break;
                case AgentMoveType.Torque: // D: Add rotation torque based on Horizontal axis input. See https://docs.unity3d.com/ScriptReference/Rigidbody2D.AddTorque.html.
                    // rb.AddTorque(-movement);
                    rb.AddTorque(new Vector3(-movement, 0, 0));
                    break;
                case AgentMoveType.ThrustCapped:
                    Vector3 move_thrust = new Vector3(movement, -grav, 0);
                    rb.AddForce(move_thrust * Time.deltaTime * speed, mode);
                    rb.AddTorque(new Vector3(-x, 0, 0));
                    break;
            }
        }
        

        triggerCall = Mathf.Max(triggerCall - Time.deltaTime, 0f); // update time delay for trigger calls

        if (debugMode && Input.GetButtonDown("Debug Reset")) {
            transform.position = init;
            rb.velocity = Vector2.zero;
        }
    }

    void FixedUpdate() {
        /* Physics loop -- runs 50x per second. */
        float x = Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed);  // clamp agent horizontal velocity
        rb.velocity = new Vector3(x, rb.velocity.y, rb.velocity.z);
    }

    private void OnTriggerEnter(Collider other) {
        /* Destroy Agent if they come into contact with a 'Kill Zone' */
        if (other.gameObject.tag == "Kill" && triggerCall == 0f && !invincible) {
            StartCoroutine("Kill");
            triggerCall = triggerTimeout;
            if (gm != null)
                gm.numDead += 1;
        }
        
        if (other.gameObject.tag == "Goal" && triggerCall == 0f) {
            StartCoroutine("Goal");
            triggerCall = triggerTimeout;
            if (gm != null)
                gm.numSaved += 1;
        }
    }

    private void OnCollisionEnter(Collision other) {
        audioSrc.clip = bumpSound;
        audioSrc.priority = 200;
        audioSrc.pitch = Random.Range(0.8f, 2f);
        audioSrc.Play();
    }

    private void OnCollisionStay(Collision other) {
        onGround = true;
    }

    private void OnCollisionExit(Collision other) {
        onGround = false;
    }

    private void OnTriggerExit(Collider other) {
        /* Kill agent if they leave the playable zone */
        // if (other.gmeObject.tag == "Play Area") { // WIP
        //     StartCoroutine("Kill");
        //     triggerCall = triggerTimeout;
        //     gm.numDead += 1;
        // }
    }

    IEnumerator Kill() {
        /* Coroutine code based on Unity manual, https://docs.unity3d.com/Manual/Coroutines.html. */

        if (cameraShake && cs != null) {
            cs.ShakeCamera(shakeAmount, shakeDuration, transform.position);
        }

        Instantiate(deathPFX, transform.position, Quaternion.identity);
        audioSrc.pitch = Random.Range(0.5f, 1.2f);
        audioSrc.clip = deathSound;
        audioSrc.priority = 100;
        audioSrc.volume = 0.3f;
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
        if (gm != null) gm.UpdateAgentCount();

        if (this.gameObject != null) Destroy(this.gameObject); /* Destroy agent GameObject after initDelay + killDelay seconds */
    }

    IEnumerator Goal() {
        /* Coroutine code based on Unity manual, https://docs.unity3d.com/Manual/Coroutines.html. */
        Instantiate(deathPFX, transform.position, Quaternion.identity);
        audioSrc.pitch = Random.Range(0.8f, 1.2f);
        audioSrc.clip = rescueSound;
        audioSrc.priority = 130;
        audioSrc.volume = 0.6f;
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
        if (gm != null)
            gm.UpdateAgentCount();

        Destroy(this.gameObject); /* Destroy agent GameObject after initDelay + killDelay seconds */
    }
}
