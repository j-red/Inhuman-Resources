using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorLight : MonoBehaviour {
    public bool active = false;

    public Color activeColor = new Color(1f, 1f, 1f);
    public Color inactiveColor = new Color(0.2f, 0.2f, 0.2f);
    private Renderer leftRender, rightRender;
    public Material leftMat, rightMat;

    [Range(0, 0.5f)]
    public float colorTime = 0.03f;

    // Start is called before the first frame update
    void Start() {
        leftRender = transform.Find("left").GetComponent<Renderer>();
        rightRender = transform.Find("right").GetComponent<Renderer>();

        leftMat = leftRender.material;
        rightMat = rightRender.material;
    }

    // Update is called once per frame
    void Update() {
        Color leftCol = leftMat.GetColor("_EmissionColor");
        Color rightCol = rightMat.GetColor("_EmissionColor");

        if (active) {
            // if active:
            leftMat.SetColor("_EmissionColor", Color.Lerp(leftCol, inactiveColor, colorTime));
            rightMat.SetColor("_EmissionColor", Color.Lerp(rightCol, activeColor, colorTime));

        } else {
            // if inactive:
            leftMat.SetColor("_EmissionColor", Color.Lerp(leftCol, activeColor, colorTime));
            rightMat.SetColor("_EmissionColor", Color.Lerp(rightCol, inactiveColor, colorTime));
        }
    }
}
