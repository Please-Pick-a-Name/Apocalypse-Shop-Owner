using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum ProjectileType {
    NONE      = 0b0000,
    EXPLOSIVE = 0b0001
}

[Serializable]
public struct ProjectileOptions {
    public ProjectileType projectileType;
    public float damage;
    public float radius;
    public bool ignoreArmour;
    public AudioClip hitSound;
    
    public GameObject prefabToSpawnOnHit;
}

[Flags]
public enum ProjectileVisualType {
    NONE     = 0b0000,
    SPRITE   = 0b0001,
    LINE     = 0b0010,
    MESH     = 0b0100,
    PARTICLE = 0b1000,
}

[Serializable]
public struct ProjectileVisualOptions {
    public ProjectileVisualType visualType;
    public Sprite sprite;
    public Gradient lineGradient;
    public Mesh mesh;
    public Material[] meshMaterials;
}

public class ProjectileManager : MonoBehaviour {
    public static ProjectileManager instance;

    public GameObject projectilePrefab;
    public Transform projectilePoolTransform;
    public int defaultPoolSize = 64;

    [Header("debug")]
    public int poolSize = 64;
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
        poolSize = defaultPoolSize;
        projectiles = new List<Projectile>(poolSize);
        for (int i = 0; i < poolSize; i++) {
            //var projectile = Instantiate(visualProjectile, projectilePool.transform);
            //projectile.SetActive(false);
            projectiles.Add(Instantiate(projectilePrefab, projectilePoolTransform).GetComponent<Projectile>());
        }
    }
    public void AddProjectile(Vector3 origin, Vector3 velocity, ProjectileOptions projectileOptions, ProjectileVisualOptions projectileVisualOptions) {
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
        projectile.Init(origin, velocity, projectileOptions, projectileVisualOptions);
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
