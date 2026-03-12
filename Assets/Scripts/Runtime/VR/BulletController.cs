using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class BulletController : XRSocketInteractor {

    public RevolverVR gunController;
    GameObject bullet;
    
    protected override void OnSelectEntered(SelectEnterEventArgs args) {
        bullet = args.interactableObject.transform.gameObject;
        Destroy(bullet);
        gunController.addAmmo();
    }

    protected override void OnSelectExited(SelectExitEventArgs args) {
        
    }
}
