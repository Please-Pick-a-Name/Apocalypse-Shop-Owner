using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(AudioSource))]
public class DoubleBarrelVR : MonoBehaviour {
    public Transform muzzleTransform;
    

    //SO data
    [SerializeField] private ProjectileOptions projectileOptions;
    [SerializeField] private ProjectileVisualOptions visualOptions;
    private AudioClip gunFireSFX;
    private float roundsPerMinute;
    private bool fireMode;
    public VRWeaponSO weaponSO;

    private bool triggerReleased = true;
    
    public float ammo = 0;
    public int pelletsPerShot = 8;
    public float pelletSpreadAngle = 4f;
    public Transform barrel;
    public bool barrelFlipped = false;
    public float barrelFlippedAngle = -9.0f, barrelUnflippedAngle = 0.0f;
    public XRSocketInteractor bulletSocketInteractor;
    public ShotgunBulletController bulletController;

    public GameObject[] bulletModels;
    private XRGrabInteractable grabInteractable;
    
    [SerializeField] private InputActionReference toggleCylinderAction;
    

    [Header("debug")]
    public Rigidbody gunRB;
    public AudioSource audioSource;
    public float roundCooldown;
    public float cd;
    public bool activated;
    

    private bool canFire;
    // Start is called before the first frame update
    void Start() {
        projectileOptions = weaponSO.projectileOptions;
        visualOptions = weaponSO.visualOptions;
        gunFireSFX = weaponSO.gunFireSFX;
        roundsPerMinute = weaponSO.roundsPerMinute;
        fireMode = weaponSO.fireMode;
        
        roundCooldown = 60f / roundsPerMinute;
        
        grabInteractable = GetComponent<XRGrabInteractable>();
        gunRB = GetComponentInParent<Rigidbody>();
    }
    
    private void OnEnable()
    {
        toggleCylinderAction.action.performed += OnToggleBarrel;
    }
    

    // Update is called once per frame
    void Update() {

        if (fireMode) {//full auto
            canFire = cd <= 0 && ammo > 0;
        } else {//semi auto
            canFire = cd <= 0 && ammo > 0 && triggerReleased && !barrelFlipped;
        }
        

        if (activated) {
            if (canFire) {
                ammo -= 1;
                
                switch (ammo) {
                    case 0: bulletModels[0].SetActive(false); 
                        bulletController.attachTransform = bulletModels[0].transform; break;
                    case 1: bulletModels[1].SetActive(false);
                        bulletController.attachTransform = bulletModels[1].transform;  break;

                }
                
                cd = roundCooldown;

                var baseVelocity = gunRB.linearVelocity;

                for (int i = 0; i < pelletsPerShot; i++) {
                    var horizontalSpread = Random.Range(-pelletSpreadAngle, pelletSpreadAngle);
                    var verticalSpread = Random.Range(-pelletSpreadAngle, pelletSpreadAngle);
                    var spreadRotation = Quaternion.AngleAxis(horizontalSpread, muzzleTransform.up) * Quaternion.AngleAxis(verticalSpread, muzzleTransform.right);
                    var pelletDirection = spreadRotation * muzzleTransform.forward;

                    ProjectileManager.instance.AddProjectile(muzzleTransform.position, baseVelocity + pelletDirection * 500f, projectileOptions, visualOptions);
                }

                audioSource.PlayOneShot(gunFireSFX);
            
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

    public void flipBarrel() {
        barrelFlipped = true;
        barrel.transform.DOLocalRotate(new Vector3(barrelFlippedAngle, 0, 0), 0.2f);
        
        if (ammo < 2) {//allow reloading only if ammo is not full
            bulletSocketInteractor.gameObject.SetActive(true);
        }
    }
    
    public void unflipBarrel() {
        barrelFlipped = false;
        barrel.transform.DOLocalRotate(new Vector3(barrelUnflippedAngle, 0, 0), 0.2f);
        bulletSocketInteractor.gameObject.SetActive(false);
    }

    public void addAmmo() {
        Debug.Log("adding ammo");
        if(ammo < 2 ) {
            ammo += 1;
        }

        switch (ammo) {
            case 1: bulletModels[0].SetActive(true); bulletController.attachTransform = bulletModels[1].transform; break;
            case 2: bulletModels[1].SetActive(true); break;
        }
        
    }
    
    private void OnToggleBarrel(InputAction.CallbackContext ctx)
    {
        if (!grabInteractable.isSelected)
            return;

        if (barrelFlipped) {
            unflipBarrel();
        } else {
            flipBarrel();
        }
    }
    
    
}
