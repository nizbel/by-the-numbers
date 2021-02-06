using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool {
        public ElementsEnum type;
        public List<GameObject> prefab;
        public int amount;
        // TODO Remove when going production
        public int remaining;
    }

    [Tooltip("Pools of objects, always make sure there's all possibilities in this list, even if not used by all days")]
    public List<Pool> pools;
    public Dictionary<int, Queue<GameObject>> poolDictionary;

    public static ObjectPool SharedInstance;

    void Awake() {
        SharedInstance = this;

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

                // Define type
                obj.GetComponent<IPooledObject>().SetPoolType(pool.type);

                objectPool.Enqueue(obj);

                // TODO Remove remaining check
                pool.remaining += 1;
            }

            poolDictionary.Add((int)pool.type, objectPool);
        }
    }

    public GameObject SpawnPooledObject(ElementsEnum type) {
        GameObject spawnedObject;

        int typeValue = (int)type;

        if (poolDictionary[typeValue].Count == 0) {
            // Add object to pool if it is empty
            spawnedObject = AddObjectToPool(type);
        } else {
            spawnedObject = poolDictionary[typeValue].Dequeue();
            spawnedObject.SetActive(true);
            spawnedObject.GetComponent<IPooledObject>().OnObjectSpawn();

            // TODO Remove remaining check
            DecreaseRemaining(type);
        }
        return spawnedObject;
    }

    public GameObject SpawnPooledObject(ElementsEnum type, Vector3 position, Quaternion rotation) {
        GameObject spawnedObject = SpawnPooledObject(type);

        // Set transform position and rotation
        spawnedObject.transform.position = position;
        spawnedObject.transform.localRotation = rotation;

        return spawnedObject;
    }

    public void ReturnPooledObject(GameObject obj) {
        // Deactivate
        obj.SetActive(false);

        ElementsEnum type = obj.GetComponent<IPooledObject>().GetPoolType();

        // Return to queue
        poolDictionary[(int)type].Enqueue(obj);
        obj.transform.parent = transform;

        // TODO Remove remaining check
        IncreaseRemaining(type);
    }

    public void ReturnPooledObject(ElementsEnum type, GameObject obj) {
        // Deactivate
        obj.SetActive(false);

        // Return to queue
        poolDictionary[(int)type].Enqueue(obj);
        obj.transform.parent = transform;

        // TODO Remove remaining check
        IncreaseRemaining(type);
    }

    public GameObject AddObjectToPool(ElementsEnum type) {
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

                // Define type
                obj.GetComponent<IPooledObject>().SetPoolType(pool.type);

                // TODO Remove this as it only serves for checking if queue changed
                pool.amount += 1;

                return obj;
            }
        }
        return null;
    }

    // TODO Remove remaining check when going to production
    void IncreaseRemaining(ElementsEnum type) {
        foreach (Pool pool in pools) {
            if (pool.type == type) {
                pool.remaining += 1;
                break;
            }
        }
    }

    // TODO Remove remaining check when going to production
    void DecreaseRemaining(ElementsEnum type) {
        foreach (Pool pool in pools) {
            if (pool.type == type) {
                pool.remaining -= 1;
                break;
            }
        }
    }
}
