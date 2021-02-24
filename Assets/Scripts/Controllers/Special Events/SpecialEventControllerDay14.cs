using UnityEngine;
using System.Collections;

public class SpecialEventControllerDay14 : MonoBehaviour {
    private const float DEFAULT_SPAWN_OFFSET_X = 2;
    private const float DEFAULT_MOVING_WALL_CHANCE = 20;
    private const float DEFAULT_ELEMENTS_DISTANCE = 2f;
    private const float REMAINING_PLAYTIME_DISTANCE_FACTOR = 75;
    private const float REMAINING_PLAYTIME_MOVING_FACTOR = 3;

    float duration;

    /*
	 * Energy wall prefab
	 */
    [SerializeField]
    GameObject energyWallPrefab;

    float defaultSpawnPositionX;

    Coroutine generation;

    // Use this for initialization
    void Start() {
        // Define default spawn position
        defaultSpawnPositionX = GameController.GetCameraXMax() + DEFAULT_SPAWN_OFFSET_X;

        // Fill duration
        duration = StageController.controller.GetCurrentMomentDuration();
        SpawnWallFormation();

        generation = StartCoroutine(GenerateWalls());
    }

    private void FixedUpdate() {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) {
            StopCoroutine(generation);
            Destroy(gameObject);
        }
    }

    void SpawnWallFormation() {
        // Spawn walls composed by random energies
        WallFormation newWallFormation = GameObject.Instantiate(energyWallPrefab).GetComponent<WallFormation>();
        // Set position
        newWallFormation.transform.position = new Vector3(defaultSpawnPositionX, Random.Range(-1.5f, 1.5f), 0);

        // Choose if walls should move
        bool shouldMove = GameController.RollChance(DEFAULT_MOVING_WALL_CHANCE + 
            Mathf.Max(0, 10 - StageController.controller.GetPlayableMomentsDuration()) * REMAINING_PLAYTIME_MOVING_FACTOR);
        if (shouldMove) { 
            newWallFormation.SetMoving(true);
            newWallFormation.SetSpeed(Vector3.up * Random.Range(1f, 2.5f));
            newWallFormation.SetSpeedDirectionTimer(Random.Range(0.4f, 0.6f));
        }
        newWallFormation.SetType(WallFormation.DOUBLE_SEQUENTIAL_ELEMENTS_TYPE);
        newWallFormation.SetDistanceType(WallFormation.ElementsDistanceType.FixedDistance);
        newWallFormation.SetDistance(DEFAULT_ELEMENTS_DISTANCE + StageController.controller.GetPlayableMomentsDuration() / REMAINING_PLAYTIME_DISTANCE_FACTOR);
    }

    IEnumerator GenerateWalls() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(ForegroundElementGenerator.DEFAULT_MIN_SPAWN_INTERVAL, ForegroundElementGenerator.DEFAULT_MAX_SPAWN_INTERVAL));
            SpawnWallFormation();
        }
    }
}