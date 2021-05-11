using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    private GameManager gm;
    public GameObject target;
    public float dx, dy;
    public int count = 10;

    public Transform targetParent = null;

    public float delay = 0;

    public bool instant = false;

    // Start is called before the first frame update
    void Start() {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if (instant) {
            for (int i = 0; i < count; i ++) {
                Instantiate(target, transform.position + new Vector3(Random.Range(-dx, dx), Random.Range(-dy, dy), 0f), Quaternion.identity, targetParent);
            }
            gm.UpdateAgentCount();
        } else {
            StartCoroutine("Spawn");
        }
    }

    IEnumerator Spawn() {
        for (int i = 0; i < count; i ++) {
            Instantiate(target, transform.position + new Vector3(Random.Range(-dx, dx), Random.Range(-dy, dy), 0f), Quaternion.identity, targetParent);
            yield return new WaitForSeconds(delay);
        }

        Destroy(this);
    }
}
