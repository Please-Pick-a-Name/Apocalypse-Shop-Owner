using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BulletUnparentingScript : MonoBehaviour
{
    private void Start() {
        this.GetComponent<XRGrabInteractable>().selectExited.AddListener(OnItemDropped);
    }

    private void Update() {
        
    }

    private void OnItemDropped(SelectExitEventArgs args) {
        this.transform.parent = null;
        this.GetComponent<Rigidbody>().isKinematic = false;
    }
}
