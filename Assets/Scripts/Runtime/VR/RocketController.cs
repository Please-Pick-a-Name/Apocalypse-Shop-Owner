using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class RocketController : XRSocketInteractor {

    public LauncherVR gunController;
    GameObject rocket;
    
    protected override void OnSelectEntered(SelectEnterEventArgs args) {
        rocket = args.interactableObject.transform.gameObject;
        Destroy(rocket);
        gunController.addAmmo();
    }

    protected override void OnSelectExited(SelectExitEventArgs args) {
        
    }
}
