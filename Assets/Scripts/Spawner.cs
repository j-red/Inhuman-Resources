using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    private GameManager gm;
    public GameObject target;
    public float dx, dy;
    public int count = 10;
    public Animator anim;

    public Transform targetParent = null;

    public float delay = 0;
    public float spawnDelay = 1f;
    private float spawnDelayIndex = 0f;

    public bool instant = false;
    public bool destroyOnEnd = true;
    public bool randomizeSpawnRot = false;
    public bool scripted = false; // if this spawner is controlled by code

    // Start is called before the first frame update
    void Start() {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        anim = GetComponent<Animator>();
        if (instant) {
            for (int i = 0; i < count; i ++) {
                Instantiate(target, transform.position + new Vector3(Random.Range(-dx, dx), Random.Range(-dy, dy), 0f), Quaternion.identity, targetParent);
            }
            gm.UpdateAgentCount();
        } else {
            if (!scripted) {
                StartCoroutine("Spawn");
            }
        }
    }

    void Update() {
        if (spawnDelayIndex <= spawnDelay) {
            spawnDelayIndex += Time.deltaTime;
        }

        if (anim.GetBool("Active")) {
            if (spawnDelayIndex >= spawnDelay) {
                spawnDelayIndex = 0f;
                StartCoroutine("Spawn");
            }
        }
    }

    IEnumerator Spawn() {
        for (int i = 0; i < count; i ++) {
            if (randomizeSpawnRot) {
                Instantiate(target, transform.position + new Vector3(Random.Range(-dx, dx), Random.Range(-dy, dy), 0f), Random.rotation, targetParent);
            } else {
                Instantiate(target, transform.position + new Vector3(Random.Range(-dx, dx), Random.Range(-dy, dy), 0f), Quaternion.identity, targetParent);
            }
            yield return new WaitForSeconds(delay);
        }

        if (destroyOnEnd) {
            Destroy(this);
        }
    }
}
