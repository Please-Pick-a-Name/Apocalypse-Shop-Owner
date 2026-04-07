using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class DialogueItem : MonoBehaviour {
	public string itemID;
	public TextMeshPro text;
	
	
	private XRGrabInteractable grabInteractable;
	public void OnItemDeliver() {
        // funny effects if we fancy
        Destroy(gameObject);
    }

	private void Awake() {
		grabInteractable = this.GetComponent<XRGrabInteractable>();
		text.gameObject.SetActive(false);
	}
	
	private void OnEnable()
	{
		if (grabInteractable != null)
		{
			grabInteractable.selectEntered.AddListener(OnGrabbed);
			grabInteractable.selectExited.AddListener(OnReleased);
		}
	}

	private void OnDisable()
	{
		if (grabInteractable != null)
		{
			grabInteractable.selectEntered.RemoveListener(OnGrabbed);
			grabInteractable.selectExited.RemoveListener(OnReleased);
		}
	}

	private void OnGrabbed(SelectEnterEventArgs args) {
		text.gameObject.SetActive(true);
	}

	private void OnReleased(SelectExitEventArgs args) {
		text.gameObject.SetActive(false);
	}
	
}