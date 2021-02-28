using UnityEngine;
using System.Collections;

public class SpecialEventControllerDay30 : MonoBehaviour {
    private const float DEFAULT_SPAWN_OFFSET_X = 2;

    private float duration;

    // Rotating formation prefab
    [SerializeField]
    GameObject rotatingFormationPrefab;

    Coroutine generation;

    float defaultSpawnPositionX;

    float lastSpawnPositionY;

    void Start() {
        // Define default spawn position
        defaultSpawnPositionX = GameController.GetCameraXMax() + DEFAULT_SPAWN_OFFSET_X;

        // Insert impossible value for y position for now
        lastSpawnPositionY = GameController.GetCameraYMax() + 1;

        // Fill duration
        duration = StageController.controller.GetCurrentMomentDuration();

        generation = StartCoroutine(GenerateRotatingFormations());
    }

    private void FixedUpdate() {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) {
            StopCoroutine(generation);
            Destroy(gameObject);
        }
    }

    void SpawnRotatingFormation() {
        RotatingFormation newFormation = GameObject.Instantiate(rotatingFormationPrefab).GetComponent<RotatingFormation>();

        newFormation.SetAmount(Formation.ElementsAmount.Low);
        newFormation.SetMaxRotatingSpeed(540);
        lastSpawnPositionY = DefinePositionY();
        newFormation.transform.position = new Vector3(defaultSpawnPositionX, lastSpawnPositionY, 0);
    }

    IEnumerator GenerateRotatingFormations() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(0.45f, 0.8f));
            SpawnRotatingFormation();
        }
    }

    float DefinePositionY() {
        if (lastSpawnPositionY > 0) {
            return Random.Range(GameController.GetCameraYMin(), -1);
        } else {
            return Random.Range(1, GameController.GetCameraYMax());
        }
    }
}