using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogueTrigger : MonoBehaviour {
    public DialogueTree tree;

    private bool playerInRange = false;

    public void TriggerDialog() {
        DialogueManager.instance.StartDialogue(tree.nodes[0]);
    }

    void Awake() {
        if (DialogueManager.instance == null) {
            Debug.LogError("DialogueManager not found in scene.");
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            playerInRange = true;
            Debug.Log("Player in range of dialogue trigger.");
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            playerInRange = false;
            Debug.Log("Player out of range of dialogue trigger.");
        }
    }

    void Update() {
        if (playerInRange && Input.GetMouseButtonDown(1)) {
            if (tree == null) {
                Debug.Log("DialogueTrigger has no DialogueTree assigned.");
                return;
            }
            
            DialogueManager.instance.StartDialogue(tree.nodes[0]);
            Debug.Log("Dialogue triggered.");
            //FindAnyObjectByType<DialogueManager>().StartDialogue(tree.nodes[0]);
        }
    }
}
