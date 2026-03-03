using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShotgunVR : MonoBehaviour {
    public Transform muzzleTransform;
    public Transform pumpTransform;

    //SO data
    private float damage;
    private AudioClip gunFireSFX;
    private float roundsPerMinute;
    public VRWeaponSO weaponSO;

    private bool triggerReleased = true;
    private bool chamberLoaded;
    private bool pumpPulledBack;
    private bool requiresPump;

    [Header("shotgun")]
    public int pelletsPerShot = 8;
    public float pelletSpreadAngle = 6f;
    public int maxTubeShells = 6;
    public int tubeShells = 5;
    public bool startChamberLoaded = true;
    public Vector3 pumpPullDirection = Vector3.back;
    public float pumpPullDistance = 0.1f;
    public float pumpReturnDistance = 0.02f;

    [Header("debug")]
    public AudioSource audioSource;
    public float roundCooldown;
    public float cd;
    public bool activated;
    public bool canFire;
    public float ammo;
    public float pumpDistance;

    private Vector3 pumpStartLocalPosition;

    // Start is called before the first frame update
    void Start() {
        damage = weaponSO.damage;
        gunFireSFX = weaponSO.gunFireSFX;
        roundsPerMinute = weaponSO.roundsPerMinute;

        roundCooldown = 60f / roundsPerMinute;
        audioSource = GetComponent<AudioSource>();

        tubeShells = Mathf.Clamp(tubeShells, 0, maxTubeShells);
        if (startChamberLoaded && tubeShells > 0) {
            tubeShells -= 1;
            chamberLoaded = true;
        }

        if (pumpTransform != null) {
            pumpStartLocalPosition = pumpTransform.localPosition;
        }
    }

    // Update is called once per frame
    void Update() {
        ammo = tubeShells + (chamberLoaded ? 1 : 0);

        TrackPump();

        canFire = cd <= 0 && chamberLoaded && triggerReleased && !requiresPump;

        if (activated) {
            if (canFire) {
                FireShot();
                triggerReleased = false;
            } else {
                // play dry ammo sound here ig
            }
        }

        cd -= Time.deltaTime;
    }

    private void FireShot() {
        chamberLoaded = false;
        requiresPump = true;
        cd = roundCooldown;

        var baseVelocity = GetComponentInParent<Rigidbody>().linearVelocity;

        for (int i = 0; i < pelletsPerShot; i++) {
            var horizontalSpread = Random.Range(-pelletSpreadAngle, pelletSpreadAngle);
            var verticalSpread = Random.Range(-pelletSpreadAngle, pelletSpreadAngle);
            var spreadRotation = Quaternion.AngleAxis(horizontalSpread, muzzleTransform.up) * Quaternion.AngleAxis(verticalSpread, muzzleTransform.right);
            var pelletDirection = spreadRotation * muzzleTransform.forward;

            ProjectileManager.instance.AddProjectile(muzzleTransform.position, baseVelocity + pelletDirection * 500f, damage);
        }

        audioSource.PlayOneShot(gunFireSFX);
    }

    private void TrackPump() {
        if (pumpTransform == null) {
            return;
        }

        var pullDirection = pumpPullDirection.normalized;
        var localOffset = pumpTransform.localPosition - pumpStartLocalPosition;
        pumpDistance = Vector3.Dot(localOffset, pullDirection);

        if (!pumpPulledBack && pumpDistance >= pumpPullDistance) {
            pumpPulledBack = true;
        }

        if (pumpPulledBack && pumpDistance <= pumpReturnDistance) {
            CompletePumpCycle();
        }
    }

    private void CompletePumpCycle() {
        pumpPulledBack = false;

        if (tubeShells > 0) {
            tubeShells -= 1;
            chamberLoaded = true;
        } else {
            chamberLoaded = false;
        }

        requiresPump = false;
    }

    public void LoadShell() {
        if (tubeShells >= maxTubeShells) {
            return;
        }

        tubeShells += 1;
    }

    public void Activated(){
        activated = true;
    }

    public void Deactivated() {
        activated = false;
        triggerReleased = true;
    }

    public void PumpCycle() {
        CompletePumpCycle();
    }
}
