using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Gun : MonoBehaviour {
    public Transform muzzleTransform;
    public float roundsPerMinute;

    public float ammo = 30;
    public AudioClip gunFireSFX;

    [Header("debug")]
    public AudioSource audioSource;
    public float roundCooldown;
    public float cd = 0;
    // Start is called before the first frame update
    void Start() {
        roundCooldown = 60f / roundsPerMinute;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        bool canFire = cd <= 0 && ammo > 0;

        if (Input.GetKey(KeyCode.Mouse0)) {
            if (canFire){
                cd = roundCooldown;
                ProjectileManager.instance.AddProjectile(muzzleTransform.position, GetComponentInParent<Rigidbody>().velocity + muzzleTransform.TransformDirection(new(0, 0, 500)));
                audioSource.PlayOneShot(gunFireSFX);
            } else {
                // play dry ammo sound here ig
            }
        }
        cd -= Time.deltaTime;
    }
}
