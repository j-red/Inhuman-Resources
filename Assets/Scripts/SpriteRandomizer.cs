using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteRandomizer : MonoBehaviour {
    public Sprite[] sprites;
    public Sprite[] alternateSprites;
    public bool useAlts = false;

    [Range(0, 12)]
    public int fps = 6;
    private float timeDelay;
    private Image img;

    // Start is called before the first frame update
    void Start() {
        timeDelay = 1f / fps;
        img = GetComponent<Image>();
        StartCoroutine("RandomSprite");
    }

    IEnumerator RandomSprite() {
        while(true) {
            if (useAlts) {
                img.sprite = alternateSprites[Random.Range(0, alternateSprites.Length)];
            } else {
                img.sprite = sprites[Random.Range(0, sprites.Length)];
            }
            yield return new WaitForSeconds(timeDelay);
        }
    }
}
