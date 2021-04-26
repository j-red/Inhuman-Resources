using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterHighlight : MonoBehaviour {
    // public ArrayList letters = new ArrayList();
    private GameManager gm;
    public GameObject a, d, esc;
    private Image ai, di;
    private Text at, dt;
    private SpriteRenderer ar, dr; // arrows

    public float minCol = 0.5f;
    private float timer = 0f;

    // Start is called before the first frame update
    void Start() {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();

        ai = a.GetComponent<Image>();
        di = d.GetComponent<Image>();
        
        at = a.transform.Find("Letter").GetComponent<Text>();
        dt = d.transform.Find("Letter").GetComponent<Text>();

        ar = a.transform.Find("Arrow").GetComponent<SpriteRenderer>();
        dr = d.transform.Find("Arrow").GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        float in_val = Input.GetAxis("Horizontal");
        float a_val = Mathf.Max(-1 * in_val, minCol);
        float d_val = Mathf.Max(in_val, minCol);

        Color ac = new Color(a_val, a_val, a_val);
        Color dc = new Color(d_val, d_val, d_val);
        
        ai.color = ac;
        at.color = ac;
        ar.color = ac;

        di.color = dc;
        dt.color = dc;
        dr.color = dc;


        float v = minCol;
        if (gm.numAgents == 0) {
            v = Mathf.SmoothStep(minCol, 1f, timer);
            timer += Time.deltaTime / 2f;
        }
        Color col = new Color(v, v, v);
        esc.GetComponent<Image>().color = col; 
        esc.transform.Find("Letter").GetComponent<Text>().color = col;
    }
}
