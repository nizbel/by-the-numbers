using UnityEngine;

public interface IPooledObject
{
    void OnObjectSpawn();

    void OnObjectDespawn();

    ElementsEnum GetPoolType();

    void SetPoolType(ElementsEnum poolType);
}
