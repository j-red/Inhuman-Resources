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
    private GameObject continueText;
    private Animator continueAnimator;

    [HeaderAttribute ("Audio")]
    public AudioClip next;
    public AudioClip speech, intro, end;
    // public float[] vols = new float[4]; // default volumes
    public float[] vols = {0.2f, 0.3f, 0.5f, 0.5f}; // default volumes
    
    [Range(0, 1f), Tooltip("Noise factor for pitch modulation.")]
    public float pitchModulation = 0.1f;


    [HeaderAttribute("Text")] // factor into level-specific scripts
    public Dialogue sdialogue;

    // Start is called before the first frame update
    void Awake() {
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
        if (dialogueActive && Input.GetButton("Primary") && !gm.isPaused) {
            // print("Slerping");
            continueAnimator.SetBool("Interacting", true);
            // continueText.transform.localScale = Vector3.Slerp(continueText.transform.localScale, new Vector3(1.25f, 1.25f, 1.25f), 0.05f);
        } else {
            continueAnimator.SetBool("Interacting", false);
            // continueText.transform.localScale = Vector3.Slerp(continueText.transform.localScale, new Vector3(1f, 1f, 1f), 0.05f);
        }

        if (dialogueActive && Input.GetButtonDown("Primary") && !gm.isPaused) {
            DisplayNextSentence();
            
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
            if (speech != null && !audioSrc.isPlaying) {
                audioSrc.clip = speech;
                audioSrc.pitch = Random.Range(tmpPitch - pitchModulation, tmpPitch + pitchModulation); // add some noise
                audioSrc.Play();
            }
            yield return new WaitForSeconds(delayTime);
        }
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
