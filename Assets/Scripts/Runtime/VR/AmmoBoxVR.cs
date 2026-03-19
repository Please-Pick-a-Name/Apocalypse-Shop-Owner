using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class AmmoBoxVR : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject bulletPrefab;
    public int ammoRemaining;

    private XRGrabInteractable currentItem;
    private bool isSpawning = false;

    private void Start() {
        SpawnNextItem();
    }

    private void SpawnNextItem() {
        if (isSpawning || ammoRemaining <= 0) {
            return;
        }
        
        GameObject newBullet = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        newBullet.transform.SetParent(spawnPoint);

        currentItem = newBullet.GetComponent<XRGrabInteractable>();
        
        newBullet.GetComponent<Rigidbody>().isKinematic = true;

        currentItem.selectEntered.AddListener(OnItemGrabbed);
    }

    private void OnItemGrabbed(SelectEnterEventArgs args) {
        if (currentItem == null) return;

        GameObject bulletGrabbed = args.interactableObject.transform.gameObject;
        
        bulletGrabbed.transform.parent = null;
        
        Rigidbody rb = bulletGrabbed.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        
        
        currentItem.selectEntered.RemoveListener(OnItemGrabbed);
        
        ammoRemaining--;
        currentItem = null;
        
        if (ammoRemaining > 0)
        {
            SpawnNextItem();
        }
    }

    public void addAmmo(int amount) {
        ammoRemaining += amount;
        
        if (currentItem == null)
        {
            SpawnNextItem();
        }
    }

    public int getAmmo() {
        return ammoRemaining;
    }
}
