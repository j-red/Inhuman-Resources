using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {
    public GameObject dialogueBox;
    public Text dialogueText;
    public bool dialogueActive;
    public Queue<string> sentences;
    private GameManager gm;
    private AudioSource audioSrc;
    public float delayTime = 0.005f;

    [HeaderAttribute ("Audio")]
    // public AudioClip soundEffect; // not used; see the clip for the audio source instead
    [Range(0, 1f), Tooltip("Noise factor for pitch modulation.")]
    public float pitchModulation = 0.1f;

    [HeaderAttribute("Text")] // factor into level-specific scripts
    public Dialogue sdialogue;

    // Start is called before the first frame update
    void Awake() {
        sentences = new Queue<string>();
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        audioSrc = GetComponent<AudioSource>();
    }

    void Start() {
        StartDialogue(sdialogue);
    }

    // Update is called once per frame
    void Update() {
        if (dialogueActive && Input.GetButtonDown("Primary") && !gm.isPaused) {
            DisplayNextSentence();
        }
    }

    public void StartDialogue (Dialogue dialogue) {
        sentences.Clear();
        dialogueActive = true;
        dialogueBox.SetActive(true);

        foreach (string sentence in dialogue.sentences) {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence() {
        if (sentences.Count == 0) {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence (string sentence) { // animates dialogue one letter at a time 
        dialogueText.text = "";
        float tmpPitch = audioSrc.pitch;
        foreach (char letter in sentence.ToCharArray()) {
            dialogueText.text += letter;

            audioSrc.pitch = tmpPitch; // reset to original
            if (audioSrc.clip != null && !audioSrc.isPlaying) {
                audioSrc.pitch = Random.Range(tmpPitch - pitchModulation, tmpPitch + pitchModulation); // add some noise
                print(audioSrc.pitch);
                audioSrc.Play();
            }
            yield return new WaitForSeconds(delayTime);
        }
    }

    void EndDialogue() {
        dialogueBox.SetActive(false);
        dialogueActive = false;

        Destroy(this.gameObject);
    }
}
