using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOutOfBounds : MonoBehaviour {
    [TooltipAttribute("Destroy an object if the absolute value of its position exceeds these values.")]
    public Vector3 absoluteBounds = new Vector3(1000, 1000, 1000);
    private GameManager gm;

    void Start() {
        GameObject g = GameObject.Find("Game Manager");
        if (g != null) gm = g.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update() {
        if (Mathf.Abs(transform.position.x) > absoluteBounds.x || Mathf.Abs(transform.position.y) > absoluteBounds.y || Mathf.Abs(transform.position.z) > absoluteBounds.z) {
            Destroy(this.gameObject);
            if (gm != null) gm.UpdateAgentCount();
            Debug.Log(this.gameObject.name + " was destroyed for being out of bounds.");
        }
    }
}
