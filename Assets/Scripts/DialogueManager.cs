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
    private float interactDelayTime;
    private GameObject continueText;
    private Animator continueAnimator;

    [HeaderAttribute ("Audio")]
    public AudioClip next;
    public AudioClip speech, intro, end;
    // public float[] vols = new float[4]; // default volumes
    public float[] vols = {0.2f, 0.3f, 0.5f, 0.5f}; // default volumes
    
    [Range(0, 1f), Tooltip("Noise factor for pitch modulation.")]
    public float pitchModulation = 0.1f;

    private bool isTyping = false;
    private string currentSentence = "";

    [HeaderAttribute("Text")] // factor into level-specific scripts
    public Dialogue sdialogue;

    private bool wasTypingWhenBtnPressed = false;
    private string interactBtn = "Primary";

    // Start is called before the first frame update
    void Awake() {
        interactDelayTime = delayTime / 2f;
        sentences = new Queue<string>();
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        audioSrc = GetComponent<AudioSource>();

        continueText = transform.Find("DialogueBox").Find("Continue Text").gameObject;
        continueAnimator = continueText.GetComponent<Animator>();
    }

    void Start() {
        StartDialogue(sdialogue);
        if (intro != null) {
            audioSrc.clip = intro;
            audioSrc.volume = vols[1];
            audioSrc.Play();
        }
    }

    // Update is called once per frame
    void Update() {
        if (dialogueActive && Input.GetButton(interactBtn) && !gm.isPaused) {
            continueAnimator.SetBool("Interacting", true);
        } else {
            continueAnimator.SetBool("Interacting", false);
        }

        if (Input.GetButtonDown(interactBtn)) {
            wasTypingWhenBtnPressed = isTyping;
        }

        if (dialogueActive && Input.GetButtonUp(interactBtn) && !gm.isPaused) {
            if (!wasTypingWhenBtnPressed) {
                DisplayNextSentence(); // only move to the next sentence if the key was pressed after the dialogue was done typing
            }
            
            if (next != null) {
                audioSrc.clip = next;
                audioSrc.volume = vols[0];
                audioSrc.pitch = 1f;
                audioSrc.Play();
            }
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
        if (isTyping) {
            // Finish this sentence first
            isTyping = false;
            dialogueText.text = currentSentence;
            StopAllCoroutines();
        } else { // clear box and move to next sentence
            if (sentences.Count == 0) {
                EndDialogue();
                return;
            }
            isTyping = true;
            string sentence = sentences.Dequeue();
            StopAllCoroutines();
            StartCoroutine(TypeSentence(sentence));
        }
    }

    IEnumerator TypeSentence (string sentence) { // animates dialogue one letter at a time 
        dialogueText.text = "";
        currentSentence = sentence;
        float tmpPitch = audioSrc.pitch;
        
        char[] chars = sentence.ToCharArray();

        string format = "", endFormat = "";
        bool formatting = false;

        // foreach (char letter in sentence.ToCharArray()) {
        for (int i = 0; i < sentence.Length; i++) {
            char letter = chars[i];

            if (letter == '<' && chars[i + 1] != '/') { // special format case -- append each letter with the enclosing format code
                // print("Open format string at index " + i.ToString());
                formatting = true;

                // dialogueText.text += letter;
                string tmp = sentence.Substring(i); // substring from current index to end of string
                int endOfFormatIndex = tmp.IndexOf('>');
                // print("End of Index Format: " + endOfFormatIndex.ToString());
                // print("Tmp: " + tmp);
                format = tmp.Substring(0, endOfFormatIndex + 1); // get format code in use

                tmp = sentence.Substring(i + endOfFormatIndex + 1); // get substr starting at end of open format code
                int startEndFormat = tmp.IndexOf('<');
                int endEndFormat = tmp.IndexOf('>') + 1;
                // print("Start End Format: " + startEndFormat.ToString());
                // print("End End Format: " + endEndFormat.ToString());
                // print("TMP: " + tmp);
                // print("TMP Length: " + tmp.Length.ToString());

                endFormat = tmp.Substring(startEndFormat, endEndFormat - startEndFormat); // get end code for format in use

                i = i + endOfFormatIndex + 1; // set char to first after open format string
                letter = chars[i];

                // print("Format String: " + format);
                // print("End format String: " + endFormat);
            }
            
            if (letter == '<' && chars[i + 1] == '/') {
                // print("Close format string at index " + i.ToString());
                formatting = false;
                string tmp = sentence.Substring(i);
                i = i + tmp.IndexOf('>') + 1;
                letter = chars[i];
            }
            
            if (formatting) { // format case -- append one letter, sandwiched by format codes
                dialogueText.text += format;
                dialogueText.text += letter;
                dialogueText.text += endFormat;
            } else { // default case -- append one letter
                dialogueText.text += letter; 
            }
            

            audioSrc.pitch = tmpPitch; // reset to original
            if (speech != null && !audioSrc.isPlaying) {
                audioSrc.clip = speech;
                audioSrc.pitch = Random.Range(tmpPitch - pitchModulation, tmpPitch + pitchModulation); // add some noise
                audioSrc.Play();
            }

            if (continueAnimator.GetBool("Interacting")) {
                yield return new WaitForSeconds(interactDelayTime);
            } else {
                yield return new WaitForSeconds(delayTime);
            }
        } // end for each char in sentence
        isTyping = false;
    }

    void EndDialogue() {
        dialogueBox.SetActive(false);
        dialogueActive = false;

        Destroy(GetComponent<Image>());

        if (end != null) {
            audioSrc.clip = next;
            audioSrc.volume = vols[3];
            audioSrc.pitch = 1f;
            audioSrc.Play();
        }

        Destroy(this.gameObject, 1); // destroy after 3 seconds
    }
}
