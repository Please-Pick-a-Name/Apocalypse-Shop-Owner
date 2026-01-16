using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieHealth : MonoBehaviour {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public float hitPoint = 100;
    public bool isAlive = true;
    [Serializable]
    public struct HealthCollider {
        public Collider collider;
        public float dmgMultiplier;

        public HealthCollider(Collider collider) : this() {
            this.collider = collider;
            this.dmgMultiplier = 1;
        }
    }
    
    public SerializableDictionary<Collider, HealthCollider> healthColliders;
    void OnValidate() {
        var colliders = GetComponentsInChildren<Collider>();
        foreach (var collider in colliders) {
            if (!healthColliders.ContainsKey(collider)) {
                healthColliders.Add(collider, new(collider));
                continue;
            }
        }
    }

    void Start() {

    }

    // TODO move to dedicated script
    [Header("WILL BE MOVED")]
    public float zombieMoveSpeed = 0.5f;
    public LineRenderer path;
    public int pathIndex = 0;
    void Update() {
        if (!isAlive) {
            return;
        }
        if (path == null) {
            return;
        }
        var pos = transform.position;
        var dir = path.transform.TransformPoint(path.GetPosition(pathIndex)) - pos;
        if (dir.magnitude < 0.1) {
            if (pathIndex == path.positionCount) { // reached the end of path
                OnReach();
                return;
            }
            pathIndex++;
            dir = path.transform.TransformPoint(path.GetPosition(pathIndex)) - pos;
        }
        pos += Time.deltaTime * zombieMoveSpeed * dir.normalized;
        pos.y = 0;
        transform.SetPositionAndRotation(pos, Quaternion.LookRotation(dir, Vector3.up));
    }

    public void OnHit(Collider collider, float damage) {
        if (!isAlive) {
            return;
        }
        HealthCollider healthCollider;
        if (!healthColliders.TryGetValue(collider, out healthCollider)) {
            return;
        }
        hitPoint -= healthCollider.dmgMultiplier * damage;
        if (hitPoint <= 0) {
            OnDeath();
        }
    }
    public void OnDeath() {
        CurrencyManager.Instance.AddCurrency(10);
        transform.DORotate(new(-90, 180, 0), 1f);
        isAlive = false;
    }
    public void OnReach() {
        CurrencyManager.Instance.RemoveCurrency(50);
        transform.DORotate(new(-90, 180, 0), 1f);
        isAlive = false;
    }
}
