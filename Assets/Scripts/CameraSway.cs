using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSway : MonoBehaviour {
    private Vector3 startPos;
    private Quaternion startRot;

    public Vector2 delta = new Vector2(0.25f, 0.25f);
    [Range(0.1f, 5f)]
    public float swayTime = 2f;
    [Range(0, 0.25f)]
    public float interpFac = 0.0025f;

    public Vector3 camSway = Vector3.zero;
    private Vector3 trueCamPos;

    public bool canSway = true;
    public bool isSwaying = false, forceSway = false;

    private Vector3 lastMousePos; 

    // Start is called before the first frame update
    void Start() {
        lastMousePos = Input.mousePosition;
        trueCamPos = transform.position;

        if (canSway || forceSway) {
            StartCoroutine("Sway");
        }
    }

    // Update is called once per frame
    void Update() {
        bool mouseMoving = Input.mousePosition == lastMousePos;

        if (!forceSway && canSway) {
            isSwaying = !(Input.GetButton("Pan") && mouseMoving);

            if (Input.GetButton("Pan") && mouseMoving) {
                StopAllCoroutines();
                isSwaying = false;
            }

            if (Input.GetButton("Pan") && !mouseMoving) {
                StopAllCoroutines();
                StartCoroutine("Sway");
            }

            if (Input.GetButtonUp("Pan")) {
                StopAllCoroutines();
                StartCoroutine("Sway");
            }
        }

        if (forceSway && !isSwaying) {
            StopAllCoroutines();
            StartCoroutine("Sway");
        }

        lastMousePos = Input.mousePosition;
    }

    IEnumerator Sway() {
        isSwaying = true;

        startPos = transform.position;
        startRot = transform.rotation;
        
        Vector3 targetPos = new Vector3(
            Random.Range(startPos.x - delta.x, startPos.x + delta.x), 
            Random.Range(startPos.y - delta.y, startPos.y + delta.y), 
            // Random.Range(startPos.z - delta.z, startPos.z + delta.z)
            startPos.z // z val
            );

        for (float i = 0; i < swayTime; i += Time.deltaTime) {
            if (Time.timeScale != 0) transform.position = Vector3.Slerp(transform.position, targetPos, interpFac);
            yield return null;
        }

        StartCoroutine("Sway");
    }
}
