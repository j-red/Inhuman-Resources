using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSFX : MonoBehaviour {
    public static AudioSource audioSrc;
    public AudioClip hover, click;
    [Range(0, 1f)]
    public float variation = 0.1f;

    // Start is called before the first frame update
    void Start() {
        audioSrc = GetComponent<AudioSource>();
    }

    public void HoverSound() {
        audioSrc.pitch = Random.Range(1 - variation, 1 + variation);
        audioSrc.PlayOneShot(hover);
    }

    public void ClickSound() {
        audioSrc.pitch = Random.Range(1 - variation, 1 + variation);
        audioSrc.PlayOneShot(click);
    }
}
