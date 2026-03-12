using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(AudioSource))]
public class RevolverVR : MonoBehaviour {
    public Transform muzzleTransform;
    

    //SO data
    private float damage;
    private AudioClip gunFireSFX;
    private float roundsPerMinute;
    private bool fireMode;
    public VRWeaponSO weaponSO;

    private bool triggerReleased = true;
    
    public float ammo = 0;
    public Transform cylinder;
    public bool cylinderFlipped = false;
    public float cylinderFlippedAngle = -130.0f, cylinderUnflippedAngle = -90.0f;
    public XRSocketInteractor bulletSocketInteractor;

    public GameObject[] bulletModels;
    private XRGrabInteractable grabInteractable;
    
    [SerializeField] private InputActionReference toggleCylinderAction;
    

    [Header("debug")]
    public AudioSource audioSource;
    public float roundCooldown;
    public float cd;
    public bool activated;
    

    private bool canFire;
    // Start is called before the first frame update
    void Start() {
        damage = weaponSO.damage;
        gunFireSFX = weaponSO.gunFireSFX;
        roundsPerMinute = weaponSO.roundsPerMinute;
        fireMode = weaponSO.fireMode;
        
        roundCooldown = 60f / roundsPerMinute;
        
        grabInteractable = GetComponent<XRGrabInteractable>();
    }
    
    private void OnEnable()
    {
        toggleCylinderAction.action.performed += OnToggleCylinder;
    }
    

    // Update is called once per frame
    void Update() {

        if (grabInteractable.isSelected) {//if is grabbed
            
        }
        
        if (Input.GetKey(KeyCode.F)) {
            if (cylinderFlipped) {
                unflipCylinder();
            } else {
                flipCylinder();
            }
        }

        if (fireMode) {//full auto
            canFire = cd <= 0 && ammo > 0;
        } else {//semi auto
            canFire = cd <= 0 && ammo > 0 && triggerReleased && !cylinderFlipped;
        }
        

        if (activated) {
            if (canFire) {
                ammo -= 1;
                
                switch (ammo) {
                    case 0: bulletModels[0].SetActive(false); break;
                    case 1: bulletModels[1].SetActive(false); break;
                    case 2: bulletModels[2].SetActive(false); break;
                    case 3: bulletModels[3].SetActive(false); break;
                    case 4: bulletModels[4].SetActive(false); break;
                    case 5: bulletModels[5].SetActive(false); break;
                }
                
                cd = roundCooldown;
                ProjectileManager.instance.AddProjectile(muzzleTransform.position, GetComponentInParent<Rigidbody>().linearVelocity + muzzleTransform.TransformDirection(new(0, 0, 500)), damage);
                SoundFXManager.instance.PlaySoundFXClip(gunFireSFX, transform, 0.2f, 0f);
                triggerReleased =  false;
            } else {
                // play dry ammo sound here ig
            }
        }
        cd -= Time.deltaTime;
    }
    
    public void Activated(){
        activated = true;
    }

    public void Deactivated() {
        activated = false;
        triggerReleased = true;
    }

    public void flipCylinder() {
        cylinderFlipped = true;
        cylinder.transform.DOLocalRotate(new Vector3(cylinderFlippedAngle, 0, 0), 0.2f);
        
        if (ammo < 6) {//allow reloading only if ammo is not full
            bulletSocketInteractor.gameObject.SetActive(true);
        }
    }
    
    public void unflipCylinder() {
        cylinderFlipped = false;
        cylinder.transform.DOLocalRotate(new Vector3(cylinderUnflippedAngle, 0, 0), 0.2f);
        bulletSocketInteractor.gameObject.SetActive(false);
    }

    public void addAmmo() {
        Debug.Log("adding ammo");
        if(ammo < 6 ) {
            ammo += 1;
        }

        switch (ammo) {
            case 1: bulletModels[0].SetActive(true); break;
            case 2: bulletModels[1].SetActive(true); break;
            case 3: bulletModels[2].SetActive(true); break;
            case 4: bulletModels[3].SetActive(true); break;
            case 5: bulletModels[4].SetActive(true); break;
            case 6: bulletModels[5].SetActive(true); break;
        }
        
    }
    
    private void OnToggleCylinder(InputAction.CallbackContext ctx)
    {
        if (!grabInteractable.isSelected)
            return;

        if (cylinderFlipped) {
            unflipCylinder();
        } else {
            flipCylinder();
        }
    }
    
    
}
