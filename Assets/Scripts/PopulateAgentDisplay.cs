using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateAgentDisplay : MonoBehaviour {
    public int numAgents = 100;
    public int width = 20, height = 5;
    public int numRecovered = 0;

    public float dx = 18f, dy = 36f;
    
    public GameObject agentIndicator;
    public Color active = new Color(1f, 1f, 1f);
    public Color inactive = new Color(0.4f, 0.4f, 0.4f);
    public Color dead = new Color(0.8f, 0.3f, 0.3f);
    public Color saved = new Color(0.4f, 0.9f, 0.2f);
    public Vector2 startPos = Vector2.zero;
    public GameObject[] indicators;

    // Start is called before the first frame update
    void Start() {
        indicators = new GameObject[numAgents];
        startPos += new Vector2(transform.position.x, transform.position.y);
        Vector3 pos = new Vector3(startPos.x, startPos.y);

        for (int j = 0; j < height; j ++) {
            for (int i = 0; i < width; i ++) {
                indicators[j * width + i] = Instantiate(agentIndicator, pos, Quaternion.identity, this.transform);
                
                // print("Instantiate at " + pos);
                pos.x += dx;
            }
            pos.x = startPos.x;
            pos.y -= dy;
        }
    }

    // public void UpdateAgentColors(int numActive, int numInactive, int numDead) {
    public void UpdateAgentColors(int numActive, int numSaved) {
        numRecovered = numSaved;

        int i = 0;
        for (; i < numActive && i < indicators.Length; i ++) {
            indicators[i].GetComponent<Image>().color = active;
        }

        for (; i < indicators.Length; i ++) {
            indicators[i].GetComponent<Image>().color = inactive;
        }

        for (i = 1; i <= numSaved && i <= indicators.Length; i ++) {
            indicators[indicators.Length - i].GetComponent<Image>().color = saved;
            // print("Index" + (indicators.Length - i));
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
