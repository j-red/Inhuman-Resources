using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentDisplay : MonoBehaviour {
    private GameManager gm;
    
    [Range(0, 100)]
    public int numLiving = 0, numSaved = 0, numDead = 0;
    public Text livingText, savedText, deadText;

    [SerializeField]
    private string formatString = "D2";
    private int maxCt = 99; // should be 10^n - 1, where N is the value following "D" in the format string above.
    
    // Start is called before the first frame update
    void Start() {
        if (GameObject.Find("Game Manager") != null) gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update() {
        
    }

    void LateUpdate() {
        numLiving = Mathf.Min(gm.numAgents, maxCt);
        numSaved = Mathf.Min(gm.numSaved, maxCt);
        numDead = Mathf.Min(gm.numDead, maxCt);

        livingText.text = numLiving.ToString(formatString);
        savedText.text = numSaved.ToString(formatString);
        deadText.text = numDead.ToString(formatString);
    }
}
