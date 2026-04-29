using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPCSpawner : MonoBehaviour {
    public static NPCSpawner instance;

    [Header("Spawn Settings")]
    public float spawnInterval = 10f;
    public Transform spawnPoint;
    
    [Header("NPC Pools")]
    public List<GameObject> regularNpcPrefabs;
    public List<GameObject> keyNpcPrefabs;
    [Header("Path Settings")]
    public Transform[] pathPoints;

    [Header("When Spawn Wave endded")]
    public UnityEvent onSpawnWaveEnd = new();
    
    [Header("debug")]
    [SerializeField] private float timer;
    [SerializeField] private int regularSpawnCount = 0;
    [SerializeField] private int keyNpcIndex = 0;

    [SerializeField] private GameObject currentNPC;

    void Awake() {
        if (instance != null & instance != this) {
            Destroy(this);
            return;
        }
        instance = this;

        //SpawnKeyNPC();
        //regularSpawnCount = 0;
        //timer = spawnInterval;
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

        if(keyNpcIndex < keyNpcPrefabs.Count) {
            SpawnNPC(keyNpcPrefabs[keyNpcIndex]);
            keyNpcIndex++; 
        }
        else {
            Debug.Log("reached the end");
            // ending 
            onSpawnWaveEnd.Invoke();
            enabled = false;
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