using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(AudioSource))]
public class GunVR : MonoBehaviour {
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
    public AmmoController currentMagazine;
    public XRSocketInteractor socketInteractor;

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

        gunRB = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {

        if (currentMagazine != null) {
            ammo = currentMagazine.getAmmo();
        } else {
            ammo = 0;
        }


        if (fireMode) {//full auto
            canFire = cd <= 0 && ammo > 0;
        } else {//semi auto
            canFire = cd <= 0 && ammo > 0 && triggerReleased;
        }
        

        if (activated) {
            if (canFire) {
                currentMagazine.ammo -= 1;
                cd = roundCooldown;
                ProjectileManager.instance.AddProjectile(muzzleTransform.position, gunRB.linearVelocity + muzzleTransform.forward * 500f, projectileOptions, visualOptions);
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
    
    
    
}
