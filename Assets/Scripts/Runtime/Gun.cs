using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Gun : MonoBehaviour {
    public Transform muzzleTransform;
    public float roundsPerMinute;
    public TextMeshProUGUI ammoCountText;

    public float damage = 1;
    public float ammo = 30;
    public float maxAmmo = 30;
    public float totalAmmo = 120;
    public AudioClip gunFireSFX;

    [Header("debug")]
    public AudioSource audioSource;
    public float roundCooldown;
    public float cd = 0;
    // Start is called before the first frame update
    void Start() {
        roundCooldown = 60f / roundsPerMinute;

    }

    // Update is called once per frame
    void Update() {
        bool canFire = cd <= 0 && ammo > 0;
        ammoCountText.SetText("Ammo: " + ammo + "/" + maxAmmo + "//" + totalAmmo);

        if (Input.GetKey(KeyCode.Mouse0)) {
            if (canFire && !PlayerController.cursorMode){
                cd = roundCooldown;
                ProjectileManager.instance.AddProjectile(muzzleTransform.position, GetComponentInParent<Rigidbody>().linearVelocity + muzzleTransform.TransformDirection(new(0, 0, 500)), damage);
                SoundFXManager.instance.PlaySoundFXClip(gunFireSFX, transform, 0.2f, 0f);
                ammo--;
            } else {
                // play dry ammo sound here ig
            }
        }
        cd -= Time.deltaTime;

        // Reload
        if (Input.GetKey(KeyCode.R) && totalAmmo > 0) {
            float reloadingAmount = maxAmmo - ammo;
            if(totalAmmo >= maxAmmo){
                ammo = maxAmmo;               
            }
            else {
                ammo = totalAmmo;
            }
            totalAmmo = totalAmmo - reloadingAmount;
            if(totalAmmo < 0) {
                totalAmmo = 0;
            }
            
        }

    }

    public void RefillAmmo(int amount) {
        totalAmmo += amount;
    }
}
