using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // Foreground elements
    public const int POSITIVE_ENERGY = 1;
    public const int NEGATIVE_ENERGY = 2;
    public const int ASTEROID = 3;
    public const int DEBRIS = 4;
    public const int GENESIS_ASTEROID = 5;
    public const int ENERGY_MINE = 6;
    public const int ENERGY_FUSE = 7;
    public const int MAGNETIC_BARRIER = 8;

    // Background elements
    public const int STAR = 21;
    public const int GALAXY = 22;
    public const int BG_DEBRIS = 23;

    [System.Serializable]
    public class Pool {
        public int type;
        public List<GameObject> prefab;
        public int amount;
        // TODO Remove when going production
        public int remaining;
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

                // TODO Remove remaining check
                pool.remaining += 1;
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

            // TODO Remove remaining check
            DecreaseRemaining(type);
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

        // TODO Remove remaining check
        IncreaseRemaining(type);
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

    // TODO Remove remaining check when going to production
    void IncreaseRemaining(int type) {
        foreach (Pool pool in pools) {
            if (pool.type == type) {
                pool.remaining += 1;
                break;
            }
        }
    }

    // TODO Remove remaining check when going to production
    void DecreaseRemaining(int type) {
        foreach (Pool pool in pools) {
            if (pool.type == type) {
                pool.remaining -= 1;
                break;
            }
        }
    }
}
