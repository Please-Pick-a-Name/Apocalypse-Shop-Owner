using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueItem : MonoBehaviour {
	public string itemID;
	public void OnItemDeliver() {
        // funny effects if we fancy
        Destroy(gameObject);
    }
}