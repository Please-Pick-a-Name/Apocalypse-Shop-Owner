using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
    public Transform muzzleTransform;
    public float roundsPerMinute;
    [Header("debug")]
    public float roundCooldown;
    public float cd = 0;
    // Start is called before the first frame update
    void Start() {
        roundCooldown = 60f / roundsPerMinute;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.Mouse0) && cd <= 0) {
            cd = roundCooldown;
            ProjectileManager.instance.AddProjectile(muzzleTransform.position, GetComponentInParent<Rigidbody>().velocity + muzzleTransform.TransformDirection(new(0, 0, 500)));
        }
        cd -= Time.deltaTime;
    }
}
