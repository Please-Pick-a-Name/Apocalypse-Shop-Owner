using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    public static ZombieManager instance;
    public HashSet<ZombieHealth> zombies;
    public int count = 0;
    public bool isZombieClear;

    void Awake() {
        if (instance != null & instance != this) {
            Destroy(this);
            return;
        }
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isZombieClear = count <= 0;
    }

    public void AddZombie(ZombieHealth zombie) {
        zombies.Add(zombie);
        count++;
    }

    public void RemoveZombie(ZombieHealth zombie) {
        if (zombies.Remove(zombie)){
            count--;
        }
    }
}
