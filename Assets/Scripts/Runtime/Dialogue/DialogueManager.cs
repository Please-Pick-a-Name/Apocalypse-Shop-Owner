using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using XNode;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class DialogueManager : MonoBehaviour {
    public static DialogueManager instance;
    public List<DialogueItem> dialogueItemsInTrigger;

    public enum SpawnLocation{
        BARRICADE_TABLE,
        COUNTER_TOP,
        BACK_TABLE
    }
    public Transform barricadeTableTransform;
    public Transform counterTopTransform;
    public Transform backTableTransform;


    public TMP_Text nameText;
    public TMP_Text sentenceText;
    public Button nextButton;
    public Button[] optionButtons;
    public TMP_Text[] optionTexts;
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

    public Node curNode;
    Queue<string> sentences = new Queue<string>();
    //AudioSource source;
    AudioClip talkingClip;
    private Coroutine sentenseCoroutine;

    public List<SimpleFollowPath> relatedNPC;

    public bool dialogueActive { get; private set; }

    void OnTriggerEnter(Collider other) {
        var dialogueItem = other.GetComponentInParent<DialogueItem>();
        if (dialogueItem == null) {
            return;
        }
        dialogueItemsInTrigger.Add(dialogueItem);

    }

    void OnTriggerExit(Collider other) {
        var dialogueItem = other.GetComponentInParent<DialogueItem>();
        if (dialogueItem == null) {
            return;
        }
        dialogueItemsInTrigger.Remove(dialogueItem);
    }

    void OnValidate() {
    }

    void Start() {
        if (instance != null & instance != this) {
            return;
        }
        instance = this;
        HideDialogueInstant();
        //source = GetComponent<AudioSource>();
    }

    void Update() {
        if (curNode is OptionDialogueNode options) {
            for (int i = 0; i < options.optionsRequireItems.Length; i++) {
                var requiredItemID = options.optionsRequireItems[i];
                if (requiredItemID == ""){ // nothing is wanted, keep interactable as true
                    optionButtons[i].interactable = true;
                    continue;
                }

                // optionButtons[i].interactable = false; // make button look weird
                bool markDisable = true;
                foreach (var dialogueItem in dialogueItemsInTrigger){
                    if (dialogueItem.itemID == requiredItemID){
                        optionButtons[i].interactable = true;
                        markDisable = false;
                        break;
                    }
                }
                if (markDisable) {
                    optionButtons[i].interactable = false;
                }
            }
        }
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
            for (int i = 0; i < options.options.Length; i++) {
                optionButtons[i].gameObject.SetActive(true);
                optionTexts[i].text = options.responses.sentences[i];
            }
            for (int i = options.options.Length; i < 4; i++) {
                optionButtons[i].gameObject.SetActive(false);
            }

            sentences.Clear();
            for (int i = 0; i < dialogue.sentences.Length; i++) {
                sentences.Enqueue(dialogue.sentences[i]);
            }

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
            for (int i = 0; i < 4; i++) {
                optionButtons[i].gameObject.SetActive(false);
            }

            sentences.Clear();
            foreach (var s in dialogue.sentences)
                sentences.Enqueue(s);

            DisplaySentence();
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

        // node type below are stright passthrough
        if (curNode is DialogueNPCSpawnNode npcNode) {
            InvokeAction(() => NPCSpawner.instance.SpawnNPC(npcNode.npcToSpawn), npcNode.delay);
        }else if (curNode is DialogueItemSpawnNode spawnNode) {
            switch (spawnNode.spawnLocation) {
                case SpawnLocation.BARRICADE_TABLE:
                    InvokeAction(() => Instantiate(spawnNode.gameObjectToSpawn, barricadeTableTransform.position, Quaternion.identity), spawnNode.delay);
                    break;
                case SpawnLocation.COUNTER_TOP:
                    InvokeAction(() => Instantiate(spawnNode.gameObjectToSpawn, counterTopTransform.position, Quaternion.identity), spawnNode.delay);
                    break;
                case SpawnLocation.BACK_TABLE:
                    InvokeAction(() => Instantiate(spawnNode.gameObjectToSpawn, backTableTransform.position, Quaternion.identity), spawnNode.delay);
                    break;
                default:
                    break;
            }
        }else if (curNode is DialogueMoneyNode moneyNode) {
            var amount = moneyNode.amountToAdd;
            if (amount < 0) {
                CurrencyManager.Instance.RemoveCurrency(-moneyNode.amountToAdd);
            } else {
                CurrencyManager.Instance.AddCurrency(moneyNode.amountToAdd);
            }
        }else if (curNode is DialogueZombieNode zombieNode) {
            if(zombieNode.spawnInterval >= 0)   ZombieSpawner.instance.spawnInterval   = zombieNode.spawnInterval;
            if(zombieNode.spawnCount >= 0)      ZombieSpawner.instance.spawnCount      = zombieNode.spawnCount;
            if(zombieNode.enemiesUnlocked >= 0) ZombieSpawner.instance.enemiesUnlocked = zombieNode.enemiesUnlocked;
        }

        NodePort port = curNode.GetOutputPort("nextNode");
        if (port == null || !port.IsConnected) {
            EndDialogue();
            return;
        }

        ProcessNode(port.Connection.node);
        return;
    }

    public void DisplayNextOption(int option) {
        if (curNode is not OptionDialogueNode optionNode)
            return;

        if (option < optionNode.optionsRequireItems.Length) {
            var requiredItemID = optionNode.optionsRequireItems[option];
            if (requiredItemID != ""){
                foreach (var dialogueItem in dialogueItemsInTrigger){
                    if (dialogueItem.itemID == requiredItemID){
                        Destroy(dialogueItem.gameObject);
                        break;
                    }
                }
            }
        }

        NodePort nextPort = optionNode.GetOutputPort($"options {option}").Connection;
        if (nextPort != null) {
            ProcessNode(nextPort.node);
        }
    }

    public void DisplayNextSimple() {
        if (curNode is not SimpleDialogueNode simpleNode)
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
                SoundFXManager.instance.PlaySoundFXClip(talkingClip, transform, 0.5f, 0f);
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

        SoundFXManager.instance.PlaySoundFXClip(panelClose, transform, 0.5f, 0f);
        HideDialogueInstant();

        dialogueActive = false;

        foreach (var npc in relatedNPC) {
            npc.talking = false; // unlock npc
            npc.moveDir = -1; // set them to leave
        }
        relatedNPC.Clear();
    }
}
