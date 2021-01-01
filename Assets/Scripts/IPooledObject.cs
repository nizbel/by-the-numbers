using UnityEngine;

public interface IPooledObject
{
    void OnObjectSpawn();

    void OnObjectDespawn();

    int GetPoolType();

    void SetPoolType(int poolType);
}
