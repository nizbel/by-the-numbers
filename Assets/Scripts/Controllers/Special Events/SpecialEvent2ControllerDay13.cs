using UnityEngine;
using System.Collections;

public class SpecialEvent2ControllerDay13 : MonoBehaviour {
    private const float DEFAULT_SPAWN_OFFSET_X = 2;
    /*
     * Speeches
     */
    [SerializeField]
    private Speech curiousFormationsSpeech;

    float duration;

    float defaultSpawnPositionX;

    /*
	 * Energy wall prefab
	 */
    [SerializeField]
    GameObject energyWallPrefab;

    Coroutine generation;

    // Use this for initialization
    void Start() {
        // Define default spawn position
        defaultSpawnPositionX = GameController.GetCameraXMax() + DEFAULT_SPAWN_OFFSET_X;

        // Fill duration
        duration = StageController.controller.GetCurrentMomentDuration();
        StartCoroutine(PlayNarrator());
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
        newWallFormation.SetType(WallFormation.RANDOM_ELEMENTS_TYPE);
        // Set position
        newWallFormation.transform.position = new Vector3(defaultSpawnPositionX, Random.Range(-1.5f, 1.5f), 0);
    }

    IEnumerator GenerateWalls() {
        while (true) {
            yield return new WaitForSeconds(ForegroundElementGenerator.DEFAULT_MIN_SPAWN_INTERVAL);
            SpawnWallFormation();
        }
    }

    IEnumerator PlayNarrator() {
        yield return new WaitForSeconds((duration/2) - 1);
        NarratorController.controller.StartMomentSpeech(curiousFormationsSpeech);
    }
}