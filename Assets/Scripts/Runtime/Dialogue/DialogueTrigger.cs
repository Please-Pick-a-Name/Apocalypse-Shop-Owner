using System;
using System.Collections;
using System.Collections.Generic;
// using System.Diagnostics;
using UnityEngine;
using XNode;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueTree tree;

    private bool playerInRange = false;
    private DialogueManager manager;

    public void TriggerDialog()
    {
        FindAnyObjectByType<DialogueManager>().StartDialogue(tree.nodes[0]);
    }

    void Awake()
    {
        manager = FindAnyObjectByType<DialogueManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            playerInRange = true;
            Debug.Log("Player in range of dialogue trigger.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) {
            playerInRange = false;
            Debug.Log("Player out of range of dialogue trigger.");
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetMouseButtonDown(1))
        {
            if (manager == null)
            {
                Debug.Log("DialogueManager not found in scene.");
                return;
            }
            if (tree == null)
            {
                Debug.Log("DialogueTrigger has no DialogueTree assigned.");
                return;
            }
            if (tree != null && manager != null)
                manager.StartDialogue(tree.nodes[0]);
                Debug.Log("Dialogue triggered.");
                //FindAnyObjectByType<DialogueManager>().StartDialogue(tree.nodes[0]);
        }
    }
}
