using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour {
    public static ProjectileManager instance;

    public GameObject projectilePrefab;
    public Transform projectilePoolTransform;
    public int poolSize = 128;

    [Header("debug")]
    public int poolIndex = 0;
    public List<Projectile> projectiles;

    // Start is called before the first frame update
    void Start() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        projectiles = new List<Projectile>(poolSize);
        for (int i = 0; i < poolSize; i++) {
            //var projectile = Instantiate(visualProjectile, projectilePool.transform);
            //projectile.SetActive(false);
            projectiles.Add(Instantiate(projectilePrefab, projectilePoolTransform).GetComponent<Projectile>());
        }
    }

    public void AddProjectile(Vector3 origin, Vector3 velocity) {
        var projectile = projectiles[poolIndex];
        var occupiedCount = 0;
        while (projectile.enabled) {
            if (occupiedCount == poolSize) {
                poolIndex = poolSize;
                IncreasePoolSize();
            }
            poolIndex++;
            poolIndex %= poolSize;
            projectile = projectiles[poolIndex];

            occupiedCount++;
        }
        projectile.Init(origin, velocity);
        poolIndex++;
        poolIndex %= poolSize;
    }

    public void IncreasePoolSize() {
        projectiles.Capacity *= 2;
        for (int i = poolSize; i < poolSize * 2; i++) {
            projectiles.Add(Instantiate(projectilePrefab, projectilePoolTransform).GetComponent<Projectile>());
        }
        poolSize *= 2;
    }
}
