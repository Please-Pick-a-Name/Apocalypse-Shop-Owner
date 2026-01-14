using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MagazineController : XRSocketInteractor
{
    public AmmoController magazine;
    public GunVR gunController;
    
    protected override void OnSelectEntered(SelectEnterEventArgs args) {
        magazine = args.interactableObject.transform.GetComponent<AmmoController>();
        gunController.currentMagazine  = magazine;
    }

    protected override void OnSelectExited(SelectExitEventArgs args) {
        magazine = null;
        gunController.currentMagazine  = magazine;
    }
}
