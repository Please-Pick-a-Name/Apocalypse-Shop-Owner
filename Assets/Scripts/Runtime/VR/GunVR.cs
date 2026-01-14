using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(AudioSource))]
public class GunVR : MonoBehaviour {
    public Transform muzzleTransform;
    public float roundsPerMinute;

    public float damage = 1;
    public float ammo = 0;    
    public XRSocketInteractor socketInteractor;
    public AmmoController currentMagazine;
    public AudioClip gunFireSFX;

    [Header("debug")]
    public AudioSource audioSource;
    public float roundCooldown;
    public float cd;
    public bool activated;
    

    

    private bool canFire;
    // Start is called before the first frame update
    void Start() {
        roundCooldown = 60f / roundsPerMinute;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {

        if (currentMagazine != null) {
            ammo = currentMagazine.getAmmo();
        } else {
            ammo = 0;
        }
        
        
        canFire = cd <= 0 && ammo > 0;

        if (activated) {
            if (canFire) {
                currentMagazine.ammo -= 1;
                
                cd = roundCooldown;
                ProjectileManager.instance.AddProjectile(muzzleTransform.position, GetComponentInParent<Rigidbody>().linearVelocity + muzzleTransform.TransformDirection(new(0, 0, 500)), damage);
                audioSource.PlayOneShot(gunFireSFX);
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
    }
    
    
    
}
