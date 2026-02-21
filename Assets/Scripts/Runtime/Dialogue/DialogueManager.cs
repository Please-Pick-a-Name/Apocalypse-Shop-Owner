using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using XNode;

[RequireComponent(typeof(AudioSource))]
public class DialogueManager : MonoBehaviour {
    public static DialogueManager instance;
    public TMP_Text nameText;
    public TMP_Text sentenceText;
    public Button nextButton, optionAButton, optionBButton;
    public TMP_Text optionAText, optionBText;
    public Image portrait;
    public AudioClip panelOpen, panelClose;
    [Space]
    //public Vector3 showPanelPos = new Vector3(0,-140,0);
    //public Vector3 hidePanelPos = new Vector3(0, -400, 0);
    public float panelAnimationTime = 1;
    public float textSpeed = 0.01f;

    [Header("Dialogue UI")]
    public GameObject dialogueBox;
    public CanvasGroup dialogueGroup;

    Node curNode;
    Queue<string> sentences = new Queue<string>();
    AudioSource source;
    AudioClip talkingClip;
    private Coroutine sentenseCoroutine;

    public List<SimpleFollowPath> relatedNPC;

    public bool dialogueActive { get; private set; }

    void Start() {
        if (instance != null & instance != this) {
            return;
        }
        instance = this;
        HideDialogueInstant();
        source = GetComponent<AudioSource>();
    }

    public void StartDialogue(Node rootNode) {
        if (dialogueActive) return;
        dialogueActive = true;

        dialogueBox.SetActive(true);
        ShowDialogue();
        /*dialogueCanvasGroup.alpha = 0f;
        dialogueCanvasGroup.interactable = true;
        dialogueCanvasGroup.blocksRaycasts = true;*/

        if (sentenseCoroutine != null){
            StopCoroutine(sentenseCoroutine);
        }

        ProcessNode(rootNode);
    }

    void ProcessNode(Node node) {
        if (sentenseCoroutine != null){
            StopCoroutine(sentenseCoroutine);
        }
        curNode = node;

        if (curNode is OptionDialogueNode options) {
            Dialogue dialogue = options.speaker;

            nameText.text = dialogue.name;
            portrait.sprite = dialogue.portrait;
            talkingClip = dialogue.talkingClip;
            sentenceText.text = "";

            nextButton.gameObject.SetActive(false);
            optionAButton.gameObject.SetActive(true);
            optionBButton.gameObject.SetActive(true);

            optionAText.text = options.responses.sentences[0];
            optionBText.text = options.responses.sentences[1];

            sentences.Clear();
            foreach (var s in dialogue.sentences)
                sentences.Enqueue(s);

            DisplaySentence();
            return;
        }

        if (curNode is SimpleDialogueNode simple) {
            Dialogue dialogue = simple.sentence;

            nameText.text = dialogue.name;
            portrait.sprite = dialogue.portrait;
            talkingClip = dialogue.talkingClip;
            sentenceText.text = "";

            nextButton.gameObject.SetActive(true);
            optionAButton.gameObject.SetActive(false);
            optionBButton.gameObject.SetActive(false);

            sentences.Clear();
            foreach (var s in dialogue.sentences)
                sentences.Enqueue(s);

            DisplaySentence();
            return;
        }

        if (curNode is DialogueNPCSpawnNode npcNode) {
            InvokeAction(() => NPCSpawner.instance.SpawnNPC(npcNode.npcToSpawn), npcNode.delay);

            NodePort port = npcNode.GetOutputPort("nextNode");
            if (port == null || !port.IsConnected) {
                EndDialogue();
                return;
            }

            ProcessNode(port.Connection.node);
            return;
        }

        if (curNode is DialogueMoneyNode moneyNode) {
            var amount = moneyNode.amountToAdd;
            if (amount < 0) {
                CurrencyManager.Instance.RemoveCurrency(-moneyNode.amountToAdd);
            } else {
                CurrencyManager.Instance.AddCurrency(moneyNode.amountToAdd);
            }

            NodePort port = moneyNode.GetOutputPort("nextNode");
            if (port == null || !port.IsConnected) {
                EndDialogue();
                return;
            }

            ProcessNode(port.Connection.node);
            return;
        }

        if (curNode is DialogueControlNode control) {
            switch (control.dialogueControl) {
                case DialogueControlNode.option.endDialogue:
                    EndDialogue();
                    break;
                case DialogueControlNode.option.continueDialogue:
                    //EndDialogue();
                    break;
                case DialogueControlNode.option.restartDialogue:
                    //EndDialogue();
                    break;
                default:
                    break;
            }
            return;
        }
    }

    public void DisplayNextOption(string option) {
        if (!(curNode is OptionDialogueNode optionNode)) {
            Debug.LogError("Option button clicked, but current node is NOT an OptionDialogueNode!");
            return;
        }

        NodePort port = option == "A" ? optionNode.GetOutputPort("optionA") : optionNode.GetOutputPort("optionB");

        if (port == null || port.Connection == null) {
            Debug.LogError("Option port not connected");
            return;
        }

        ProcessNode(port.Connection.node);
    }


    public void DisplayNextSimple() {
        if (!(curNode is SimpleDialogueNode simpleNode))
            return;

        NodePort port = simpleNode.GetOutputPort("nextNode");

        if (port == null || !port.IsConnected) {
            EndDialogue();
            return;
        }

        ProcessNode(port.Connection.node);
    }

    public void DisplaySentence() {
        if (sentenseCoroutine != null){
            StopCoroutine(sentenseCoroutine);
        }
        sentenseCoroutine = StartCoroutine(RenderSentence(sentences.Dequeue()));
    }

    IEnumerator RenderSentence(string sentence) {
        sentenceText.text = "";
        char[] letters = sentence.ToCharArray();
        for (int i = 0; i < letters.Length; i++) {
            sentenceText.text += letters[i];
            if (i % 4 == 0)
                source.PlayOneShot(talkingClip);
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void InvokeAction(Action action, float delay) {
        StartCoroutine(InvokeRoutine(action, delay));
    }

    IEnumerator InvokeRoutine(Action action, float delay) {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }


    public void ShowDialogue() {
        dialogueGroup.alpha = 1f;
        dialogueGroup.interactable = true;
        dialogueGroup.blocksRaycasts = true;
    }

    public void HideDialogueInstant() {
        dialogueGroup.alpha = 0f;
        dialogueGroup.interactable = false;
        dialogueGroup.blocksRaycasts = false;
    }

    public void EndDialogue() {
        if (sentenseCoroutine != null){
            StopCoroutine(sentenseCoroutine);
        }

        source.PlayOneShot(panelClose);
        HideDialogueInstant();

        dialogueActive = false;

        foreach (var npc in relatedNPC) {
            npc.talking = false; // unlock npc
            npc.moveDir = -1; // set them to leave
        }
        relatedNPC.Clear();
    }
}
