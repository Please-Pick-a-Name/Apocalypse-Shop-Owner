using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour {
    public static NPCSpawner instance;

    [Header("Spawn Settings")]
    public float spawnInterval = 10f;
    public Transform spawnPoint;
    private float timer;
    
    [Header("NPC Pools")]
    public List<GameObject> regularNpcPrefabs;
    public List<GameObject> keyNpcPrefabs;
    [Header("Path Settings")]
    public Transform[] pathPoints;

    private int regularSpawnCount = 0;
    private int keyNpcIndex = 0;

    private GameObject currentNPC;

    void Awake() {
        if (instance != null & instance != this) {
            Destroy(this);
            return;
        }
        instance = this;

        SpawnKeyNPC();
        regularSpawnCount = 0;
        timer = spawnInterval;
    }

    void Update() {
        if (currentNPC != null) {
            return; 
        }
        timer -= Time.deltaTime;
        if (timer <= 0) {
            DetermineAndSpawn();
            timer = spawnInterval;
        }
    }

    private void DetermineAndSpawn() {
        if (regularSpawnCount >= 3) {
            SpawnKeyNPC();
            regularSpawnCount = 0;
        } else {
            SpawnRegularNPC();
            regularSpawnCount++;
        }
    }

    public void SpawnRegularNPC() {
        if (regularNpcPrefabs.Count == 0) return;

        int randomIndex = Random.Range(0,regularNpcPrefabs.Count);
        SpawnNPC(regularNpcPrefabs[randomIndex]);
    }

    public void SpawnKeyNPC() {
        if(keyNpcPrefabs.Count == 0) return;

        SpawnNPC(keyNpcPrefabs[keyNpcIndex]);
        if(keyNpcIndex < keyNpcPrefabs.Count) {
            keyNpcIndex = keyNpcIndex + 1; 
        }
        else {
            Debug.Log("reached the end");
            // ending 
        }
        
    }

    public SimpleFollowPath SpawnNPC(GameObject myNpcPrefab) {
        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
        currentNPC = Instantiate(myNpcPrefab, position, Quaternion.identity);

        SimpleFollowPath followScript = currentNPC.GetComponentInChildren<SimpleFollowPath>();

        if (followScript != null) {
            followScript.waypoints = pathPoints;
        }
        return followScript;
    }
}