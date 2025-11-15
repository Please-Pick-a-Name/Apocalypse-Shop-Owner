using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Projectile : MonoBehaviour {
    public Vector3 pos;
    public Vector3 vel;
    public bool physicsEnabled = false;
    public LineRenderer lineRenderer;
    public void Init(Vector3 origin, Vector3 velocity) {
        pos = origin;
        vel = velocity;
        enabled = true;
        physicsEnabled = true;
        lineRenderer.enabled = true;
    }
    public void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
        enabled = false;
    }
    public void FixedUpdate() {
        if (!physicsEnabled) {
            return;
        }
        var dt = Time.fixedDeltaTime;
        var maxDistance = vel.magnitude * dt;
        if (Physics.Raycast(pos, vel, out RaycastHit hitInfo, maxDistance)) {
            lineRenderer.SetPosition(0, pos);
            lineRenderer.SetPosition(1, hitInfo.point);
            OnProjectileHit(hitInfo.collider, hitInfo.point);
            Invoke(nameof(Reset), 0.1f);

            return;
        }
        var newVel = vel + Physics.gravity * dt;
        var newPos = pos + vel * dt;

        if (newPos.y <= -10) {
            lineRenderer.enabled = false;
            enabled = false;
            return;
        }

        lineRenderer.SetPosition(0, pos);
        lineRenderer.SetPosition(1, newPos);

        vel = newVel;
        pos = newPos;
    }
    public void OnProjectileHit(Collider hitTarget, Vector3 hitPosition) {
        physicsEnabled = false;
    }
    private void Reset() {
        lineRenderer.enabled = false;
        enabled = false;
    }
}
