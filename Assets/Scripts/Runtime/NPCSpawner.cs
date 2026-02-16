using UnityEngine;

public class NPCSpawner : MonoBehaviour {
    public static NPCSpawner instance;
    [Header("Spawn Settings")]
    public bool manualSpawn = true;
    public GameObject npcPrefab;
    public GameObject introNpcPrefab;
    public Transform spawnPoint;
    public float cd;
    [Header("Path Settings")]
    public Transform[] pathPoints;

    void Awake() {
        if (instance != null & instance != this) {
            return;
        }
        instance = this;

        if (manualSpawn) {
            SpawnNPC(introNpcPrefab);
        }
    }

    void Update() {
        if (manualSpawn) {
            return;
        }
        if (cd <= 0) {
            SpawnNPC();
            cd += 3;
        }
        cd -= Time.deltaTime;
    }

    public void SpawnNPC() {
        SpawnNPC(npcPrefab);
    }

    public SimpleFollowPath SpawnNPC(GameObject myNpcPrefab) {
        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject spawnedNPC = Instantiate(myNpcPrefab, position, Quaternion.identity);

        SimpleFollowPath followScript = spawnedNPC.GetComponentInChildren<SimpleFollowPath>();

        if (followScript != null) {
            followScript.waypoints = pathPoints;
        }
        return followScript;
    }
}