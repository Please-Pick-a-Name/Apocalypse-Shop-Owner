using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using XNode;
using System.Diagnostics;
using System.Security.AccessControl;

public class DialogueManager : MonoBehaviour
{
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

    public bool dialogueActive { get; private set; }

    void Start()
    {
        HideDialogueInstant();
        source = GetComponent<AudioSource>();
    }

    public void StartDialogue(Node rootNode)
    {
        if (dialogueActive) return;
        dialogueActive = true;

        dialogueBox.SetActive(true);
        ShowDialogue();
        /*dialogueCanvasGroup.alpha = 0f;
        dialogueCanvasGroup.interactable = true;
        dialogueCanvasGroup.blocksRaycasts = true;*/

        PlayerController.instance.LockForDialogue();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerController.instance.cursorMode = true;

        StopAllCoroutines();

        ProcessNode(rootNode);

        /*curNode = rootNode;
        //option node
        if (curNode.GetType() == typeof(OptionDialogueNode))
        {
            //load node for speaker
            OptionDialogueNode options = curNode as OptionDialogueNode;
            Dialogue dialogue = options.speaker;
            
            //set panel
            nameText.text = dialogue.name;
            portrait.sprite = dialogue.portrait;
            talkingClip = dialogue.talkingClip;
            sentenceText.text = "";

            //set buttons
            nextButton.gameObject.SetActive(false);
            optionAButton.gameObject.SetActive(true);
            optionBButton.gameObject.SetActive(true);
            
            //load responses
            optionAText.text = options.responses.sentences[0];
            optionBText.text = options.responses.sentences[1];

            sentences.Clear();
            for (int i = 0; i < dialogue.sentences.Length; i++)
            {
                sentences.Enqueue(dialogue.sentences[i]);
            }

            source.PlayOneShot(panelOpen);
            //transform.DOLocalMove(showPanelPos, panelAnimationTime).OnComplete(() => DisplaySentence());
        }
        //simple node
        else if (curNode.GetType() == typeof(SimpleDialogueNode))
        {
            //load node for speaker
            SimpleDialogueNode simple = curNode as SimpleDialogueNode;
            Dialogue dialogue = simple.sentence;
            
            //set panel
            nameText.text = dialogue.name;
            portrait.sprite = dialogue.portrait;
            talkingClip = dialogue.talkingClip;
            sentenceText.text = "";

            //set buttons
            nextButton.gameObject.SetActive(true);
            optionAButton.gameObject.SetActive(false);
            optionBButton.gameObject.SetActive(false);

            sentences.Clear();
            for (int i = 0; i < dialogue.sentences.Length; i++)
            {
                sentences.Enqueue(dialogue.sentences[i]);
            }

            source.PlayOneShot(panelOpen);
            //transform.DOLocalMove(showPanelPos, panelAnimationTime).OnComplete(() => DisplaySentence());
        }
        //control node;
        else
        {
            //load node for speaker
            DialogueControlNode control = curNode as DialogueControlNode;
            
            if (control.dialogueControl == DialogueControlNode.option.endDialogue)
            {
                EndDialogue();
            }
            else if (control.dialogueControl == DialogueControlNode.option.continueDialogue)
            {
                //continue Dialogue
            }
            else
            {
                //restart Dialogue
            }
        }*/
    }

    void ProcessNode(Node node)
    {
        StopAllCoroutines();
        curNode = node;

        if (curNode is OptionDialogueNode options)
        {
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

        if (curNode is SimpleDialogueNode simple)
        {
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
    
        if (curNode is DialogueControlNode control)
        {
            if (control.dialogueControl == DialogueControlNode.option.endDialogue)
                EndDialogue();
        }
    }

    public void DisplayNextOption(string option)
    {
        if (!(curNode is OptionDialogueNode optionNode))
        {
            UnityEngine.Debug.LogError("Option button clicked, but current node is NOT an OptionDialogueNode!");
            return;
        }

        NodePort port = option == "A" ? optionNode.GetOutputPort("optionA") : optionNode.GetOutputPort("optionB");

        if (port == null || port.Connection == null)
        {
            UnityEngine.Debug.LogError("Option port not connected");
            return;
        }

        /*if (option == "A")
        {
            OptionDialogueNode optionNode = curNode as OptionDialogueNode;
            
            NodePort portA = optionNode.GetOutputPort("optionA").Connection;
            
            if (portA != null)
            {
                curNode = portA.node;
            }
            port = optionNode.GetOutputPort("optionA");
        }
        else
        {
            OptionDialogueNode optionNode = curNode as OptionDialogueNode;
            NodePort portB = optionNode.GetOutputPort("optionB").Connection;
            
            if (portB != null)
            {
                curNode = portB.node;
            }
            port = optionNode.GetOutputPort("optionB");
        }
        
        curNode = port.Connection.node;
        StartDialogue(curNode);*/

        ProcessNode(port.Connection.node);
    }


    public void DisplayNextSimple()
    {
        /*SimpleDialogueNode simpleNode = curNode as SimpleDialogueNode;
            
        NodePort port = simpleNode.GetOutputPort("nextNode").Connection;
            
        if (port != null)
        {
            curNode = port.node;
        }
        
        StartDialogue(curNode);*/

        if (!(curNode is SimpleDialogueNode simpleNode))
        return;

        NodePort port = simpleNode.GetOutputPort("nextNode");

        if (port == null || !port.IsConnected)
        {
            EndDialogue();
            return;
        }

        ProcessNode(port.Connection.node);
    }

    public void DisplaySentence()
    {
        StopAllCoroutines();
        StartCoroutine(RenderSentence(sentences.Dequeue()));
    }

    IEnumerator RenderSentence(string sentence)
    {
        sentenceText.text = "";
        char[] letters = sentence.ToCharArray();
        for (int i = 0; i < letters.Length; i++)
        {
            sentenceText.text += letters[i];
            if(i % 4 == 0)
                source.PlayOneShot(talkingClip);
            yield return new WaitForSeconds(textSpeed);
        }
    }

    public void ShowDialogue()
    {
        dialogueGroup.alpha = 1f;
        dialogueGroup.interactable = true;
        dialogueGroup.blocksRaycasts = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerController.instance.cursorMode = true;

    }

    public void HideDialogueInstant()
    {
        dialogueGroup.alpha = 0f;
        dialogueGroup.interactable = false;
        dialogueGroup.blocksRaycasts = false;
    }

    public void EndDialogue()
    {
        /*dialogueActive = false;
        PlayerController.instance.UnlockFromDialogue();

        StopAllCoroutines();
        source.PlayOneShot(panelClose);
        transform.DOLocalMove(hidePanelPos, panelAnimationTime);*/

        StopAllCoroutines();

        source.PlayOneShot(panelClose);
        HideDialogueInstant();

        PlayerController.instance.UnlockFromDialogue();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerController.instance.cursorMode = false;

        dialogueActive = false;

    }
}
