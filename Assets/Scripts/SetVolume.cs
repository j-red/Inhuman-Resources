using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour {
    public string targetGroup = "Master Volume";
    public AudioMixer mixer;

    /* Proper logarithmic volume falloff -- see https://gamedevbeginner.com/the-right-way-to-make-a-volume-slider-in-unity-using-logarithmic-conversion/. */
    public void SetLevel (float value) {
        mixer.SetFloat(targetGroup, Mathf.Log10(value) * 20);
    }
}
