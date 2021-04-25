using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public GameObject target;
    public float dx, dy;
    public int count = 10;

    public Transform targetParent = null;

    public float delay = 0;

    // Start is called before the first frame update
    void Start() {
        StartCoroutine("Spawn");
    }

    // Update is called once per frame
    void Update() {

    }

    IEnumerator Spawn() {
        for (int i = 0; i < count; i ++) {
            Instantiate(target, transform.position + new Vector3(Random.Range(-dx, dx), Random.Range(-dy, dy), 0f), Quaternion.identity, targetParent);
            yield return new WaitForSeconds(delay);
        }

        Destroy(this);
    }
}
