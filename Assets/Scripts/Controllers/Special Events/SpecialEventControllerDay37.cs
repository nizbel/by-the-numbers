using UnityEngine;
using System.Collections;

public class SpecialEventControllerDay37 : MonoBehaviour {
    private const float DEFAULT_SPAWN_OFFSET_X = 6f;

    GenesisAsteroid genesisAsteroid = null;

    bool calledCoroutine = false;

    void Start() {
        // Spawn genesis asteroid farther from the player
        genesisAsteroid = ObjectPool.SharedInstance.SpawnPooledObject(ElementsEnum.GENESIS_ASTEROID, 
            Vector3.right * (GameController.GetCameraXMax() + DEFAULT_SPAWN_OFFSET_X), 
            GameObjectUtil.GenerateRandomRotation()).GetComponent<GenesisAsteroid>();
        genesisAsteroid.SetIsDestructibleNow(false);
        genesisAsteroid.GetComponent<IMovingObject>().SetSpeed(Vector3.right * PlayerController.controller.GetSpeed() * 0.75f);
    }

    private void FixedUpdate() {
        // Prepare to destroy event once genesis asteroid hits screen left margin 
        if (!calledCoroutine && genesisAsteroid.transform.position.x <= GameController.GetCameraXMin()) {
            StartCoroutine(SetGenesisAsteroidRemovable());
            calledCoroutine = true;
        }
    }

    IEnumerator SetGenesisAsteroidRemovable() {
        yield return new WaitForSeconds(5f);
        genesisAsteroid.SetIsDestructibleNow(true);
        Destroy(gameObject);
    }

}