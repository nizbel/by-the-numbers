using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public const int POSITIVE_ENERGY = 1;
    public const int NEGATIVE_ENERGY = 2;
    public const int ASTEROID = 3;

    [System.Serializable]
    public class Pool {
        public int type;
        public List<GameObject> prefab;
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
                GameObject obj;
                if (pool.prefab.Count == 1) {
                    obj = Instantiate(pool.prefab[0]);
                } else {
                    obj = Instantiate(pool.prefab[Random.Range(0, pool.prefab.Count)]);
                }
                obj.SetActive(false);
                obj.transform.parent = transform;
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.type, objectPool);
        }
    }

    public GameObject SpawnPooledObject(int type) {
        GameObject spawnedObject;

        if (poolDictionary[type].Count == 0) {
            // Add object to pool if it is empty
            spawnedObject = AddObjectToPool(type);
        } else {
            spawnedObject = poolDictionary[type].Dequeue();
            spawnedObject.SetActive(true);
            spawnedObject.GetComponent<IPooledObject>().OnObjectSpawn(); 
        }
        return spawnedObject;
    }

    public GameObject SpawnPooledObject(int type, Vector3 position, Quaternion rotation) {
        GameObject spawnedObject = SpawnPooledObject(type);

        // Set transform position and rotation
        spawnedObject.transform.position = position;
        spawnedObject.transform.localRotation = rotation;

        return spawnedObject;
    }

    public void ReturnPooledObject(int type, GameObject obj) {
        poolDictionary[type].Enqueue(obj);
        obj.transform.parent = transform;
    }

    public GameObject AddObjectToPool(int type) {
        foreach (Pool pool in pools) {
            if (pool.type == type) {
                GameObject obj;
                if (pool.prefab.Count == 1) {
                    obj = Instantiate(pool.prefab[0]);
                }
                else {
                    obj = Instantiate(pool.prefab[Random.Range(0, pool.prefab.Count)]);
                }
                obj.transform.parent = transform;

                // TODO Remove this as it only serves for checking if queue changed
                pool.amount += 1;
                return obj;
            }
        }
        return null;
    }
}
