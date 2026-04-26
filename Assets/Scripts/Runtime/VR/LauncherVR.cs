using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(AudioSource))]
public class LauncherVR : MonoBehaviour {
    public Transform muzzleTransform;
    

    //SO data
    private float damage;
    private AudioClip gunFireSFX;
    private float roundsPerMinute;
    private bool fireMode;
    public VRWeaponSO weaponSO;

    private bool triggerReleased = true;
    
    public float ammo = 0;
    
    public XRSocketInteractor bulletSocketInteractor;

    public GameObject rocketModel;
    private XRGrabInteractable grabInteractable;
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
    

    // Update is called once per frame
    void Update() {

        if (fireMode) {//full auto
            canFire = cd <= 0 && ammo > 0;
        } else {//semi auto
            canFire = cd <= 0 && ammo > 0 && triggerReleased;
        }
        

        if (activated) {
            if (canFire) {
                ammo -= 1;
                rocketModel.SetActive(false);
                
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

    public void addAmmo() {
        Debug.Log("adding ammo");
        if(ammo < 1 ) {
            ammo += 1;
            rocketModel.SetActive(true);
        }
        
        
    }
    
    
    
}
