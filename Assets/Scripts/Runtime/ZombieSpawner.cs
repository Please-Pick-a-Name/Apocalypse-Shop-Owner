using Unity.Mathematics;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour {
    BoxCollider areaToSpawn;
    public GameObject hordeGroup;
    public GameObject pathGroup;
    public GameObject prefabToSpawn;
    public float spawnInterval = 5f;
    public int spawnCount = 1;
    
    [Header("auto generated")]
    public LineRenderer[] paths;
    public int pathCount = 0;

    [Header("debug")]
    public float cd = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnValidate() {
        paths = pathGroup.GetComponentsInChildren<LineRenderer>();
        pathCount = paths.Length;
    }
    void Start() {
        areaToSpawn = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update() {
        if (cd <= 0) {
            for (int count = 0; count < spawnCount; count++) {
                var path = paths[UnityEngine.Random.Range(0, pathCount)];
                var zombieGO = Instantiate(prefabToSpawn, path.transform.TransformPoint(path.GetPosition(0)), quaternion.identity, hordeGroup.transform);
                var zombieHealth = zombieGO.GetComponent<ZombieHealth>();
                zombieHealth.path = path;
            }
            cd += spawnInterval;
        }
        cd -= Time.deltaTime;
    }

    private static Vector3 GetRandomPointInBox(BoxCollider box) {
        return box.transform.TransformPoint(new(UnityEngine.Random.Range(0, box.size.x), UnityEngine.Random.Range(0, box.size.y), UnityEngine.Random.Range(0, box.size.z)));
    }
}
