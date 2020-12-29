using UnityEngine;

public interface IPooledObject
{
    void OnObjectSpawn();

    void OnObjectDespawn();

    void SetPoolType(int poolType);
}
