using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSway : MonoBehaviour {
    private Vector3 startPos;
    private Quaternion startRot;

    public Vector3 delta = new Vector3(0.25f, 0.25f, 0.1f);
    [Range(0.1f, 5f)]
    public float swayTime = 2f;
    [Range(0, 0.25f)]
    public float interpFac = 0.0025f;

    // Start is called before the first frame update
    void Start() {
        startPos = transform.position;
        startRot = transform.rotation;

        StartCoroutine("Sway");
    }

    // Update is called once per frame
    void Update() {
        
    }

    IEnumerator Sway() {
        Vector3 targetPos = new Vector3(Random.Range(startPos.x - delta.x, startPos.x + delta.x), Random.Range(startPos.y - delta.y, startPos.y + delta.y), Random.Range(startPos.z - delta.z, startPos.z + delta.z));
        for (float i = 0; i < swayTime; i += Time.deltaTime) {
            transform.position = Vector3.Slerp(transform.position, targetPos, interpFac);
            yield return null;
        }
        StartCoroutine("Sway");
    }
}
