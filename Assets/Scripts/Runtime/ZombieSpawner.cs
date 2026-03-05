using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class ZombieSpawner : MonoBehaviour {
    BoxCollider areaToSpawn;
    public GameObject hordeGroup;
    public GameObject pathGroup;
    public GameObject[] enemyList;
    public GameObject prefabToSpawn;
    public float spawnInterval = 5f;
    public int spawnCount = 1;
    public int escalationLevel = 1; //higher escalationLevel lowers spawnInterval and adds new enemy types
    public float escalationInterval = 120f;  //Interval that escalationLevel increases
    public float escalationTimer;
    public int enemiesUnlocked = 1;
    
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
        escalationTimer = escalationInterval;
    }

    // Update is called once per frame
    void Update() {
        if (escalationTimer <= 0) {
            escalationLevel++;//increase difficulty
            spawnInterval *= 0.9f; // lower spawn interval
            if (escalationLevel % 5 == 0 && enemiesUnlocked < enemyList.Length) {//unlock new enemy type every 5 difficulty escalations
                        enemiesUnlocked++;
            }
            escalationTimer += escalationInterval;
        }
        
        
        if (cd <= 0) {
            for (int count = 0; count < spawnCount; count++) {
                var path = paths[UnityEngine.Random.Range(0, pathCount)];
                prefabToSpawn = enemyList[UnityEngine.Random.Range(0, enemiesUnlocked)];
                var zombieGO = Instantiate(prefabToSpawn, path.transform.TransformPoint(path.GetPosition(0)), quaternion.identity, hordeGroup.transform);
                var zombieHealth = zombieGO.GetComponent<ZombieHealth>();
                zombieHealth.path = path;
            }
            cd += spawnInterval;
        }
        cd -= Time.deltaTime;
        escalationTimer -= Time.deltaTime;
        
    }

    private static Vector3 GetRandomPointInBox(BoxCollider box) {
        return box.transform.TransformPoint(new(UnityEngine.Random.Range(0, box.size.x), UnityEngine.Random.Range(0, box.size.y), UnityEngine.Random.Range(0, box.size.z)));
    }
}
