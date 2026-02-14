using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject npcPrefab;
    public Transform spawnPoint; 
    public float cd;
    [Header("Path Settings")]
    public Transform[] pathPoints; 

    void Update()
    {
        if (cd <= 0) {
            SpawnNPC();
            cd += 3;
        }
        cd -= Time.deltaTime;
    }

    public void SpawnNPC()
    {

        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject spawnedNPC = Instantiate(npcPrefab, position, Quaternion.identity);

        SimpleFollowPath followScript = spawnedNPC.GetComponent<SimpleFollowPath>();
        
        if (followScript != null)
        {
            followScript.waypoints = pathPoints;
        }
    }
}