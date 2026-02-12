using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieHealth : MonoBehaviour {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public float hitPoint = 100;
    public bool isAlive = true;

    public Animator anim;
    
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
    public float attackCooldown = 2.5f; // seconds between attacks
    private float attackCooldownTimer = 0f;
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
            if (pathIndex >= path.positionCount - 1) { // reached the end of path
                //Attack Logic Here
                if(attackCooldownTimer > 0) attackCooldownTimer -= Time.deltaTime;
                if (attackCooldownTimer <= 0) {
                    attackCooldownTimer = attackCooldown;
                    int attackAnimation = Random.Range(1, 3);
                    switch (attackAnimation) {
                        case 1: anim.SetTrigger("Attack1"); break;
                        case 2: anim.SetTrigger("Attack2"); break;
                    }
                }
                
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
        } else {
            StartCoroutine(PlayHitReaction());
            anim.SetTrigger("Hit");
        }
    }
    public void OnDeath() {
        CurrencyManager.Instance.AddCurrency(10);
        isAlive = false;
        int deathAnimation = Random.Range(1, 3);
        switch (deathAnimation) {
            case 1: anim.SetTrigger("Death1"); break;
            case 2: anim.SetTrigger("Death2"); break;
        }
    }
    public void OnReach() {
        CurrencyManager.Instance.RemoveCurrency(50);
        
        //transform.DORotate(new(-90, 180, 0), 1f);
        //isAlive = false;
    }
    
    IEnumerator PlayHitReaction()
    {
        anim.SetLayerWeight(1, 0.8f); // hit layer
        yield return new WaitForSeconds(0.15f);
        anim.SetLayerWeight(1, 0f);
    }
}
