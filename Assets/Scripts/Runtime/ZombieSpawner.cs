using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ZombieSpawner : MonoBehaviour {
    BoxCollider areaToSpawn;
    public GameObject hordeGroup;
    public GameObject prefabToSpawn;
    public float spawnInterval = 5f;
    public int spawnCount = 1;
    [Header("debug")]
    public float cd = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        areaToSpawn = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update() {
        if (cd <= 0) {
            var rndPos = GetRandomPointInBox(areaToSpawn);
            for (int i = 0; i < spawnCount; i++) {
                Instantiate(prefabToSpawn, rndPos, quaternion.identity, hordeGroup.transform);
            }
            cd += spawnInterval;
        }
        cd -= Time.deltaTime;
    }

    private static Vector3 GetRandomPointInBox(BoxCollider box) {
        return box.transform.TransformPoint(new(UnityEngine.Random.Range(0, box.size.x), UnityEngine.Random.Range(0, box.size.y), UnityEngine.Random.Range(0, box.size.z)));
    }
}
