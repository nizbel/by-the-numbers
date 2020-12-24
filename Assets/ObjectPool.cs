using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public const int POSITIVE_ENERGY = 1;
    public const int NEGATIVE_ENERGY = 2;

    [System.Serializable]
    public class Pool {
        public int type;
        public GameObject prefab;
        public int amount;
    }

    public List<Pool> pools;
    public Dictionary<int, Queue<GameObject>> poolDictionary;

    public static ObjectPool SharedInstance;

    void Awake() {
        SharedInstance = this;
    }

    void Start() {
        poolDictionary = new Dictionary<int, Queue<GameObject>>();

        foreach (Pool pool in pools) {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.amount; i++) {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.parent = transform;
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.type, objectPool);
        }
    }

    public GameObject SpawnPooledObject(int type) {
        GameObject spawnedObject = poolDictionary[type].Dequeue();
        spawnedObject.SetActive(true);
        spawnedObject.GetComponent<IPooledObject>().OnObjectSpawn();
        return spawnedObject;
    }

    public GameObject SpawnPooledObject(int type, Vector3 position, Quaternion rotation) {
        GameObject spawnedObject = poolDictionary[type].Dequeue();
        
        // Activate and call spawn method
        spawnedObject.SetActive(true);
        spawnedObject.GetComponent<IPooledObject>().OnObjectSpawn();

        // Set transform position and rotation
        spawnedObject.transform.position = position;
        spawnedObject.transform.localRotation = rotation;

        return spawnedObject;
    }

    public void ReturnPooledObject(int type, GameObject obj) {
        poolDictionary[type].Enqueue(obj);
        obj.transform.parent = transform;
    }
}
