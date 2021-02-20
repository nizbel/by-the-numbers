using UnityEngine;
using System.Collections;

public class SpecialEventControllerDay14 : MonoBehaviour {
    private int eventCode;

    public int EventCode { get => eventCode; set => eventCode = value; }

    float duration;

    /*
	 * Energy wall prefab
	 */
    [SerializeField]
    GameObject energyWallPrefab;

    Coroutine generation;

    // Use this for initialization
    void Start() {
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
        newWallFormation.SetMoving(GameController.RollChance(20));
        newWallFormation.SetType(WallFormation.RANDOM_ELEMENTS_TYPE);
    }

    IEnumerator GenerateWalls() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(ForegroundElementGenerator.DEFAULT_MIN_SPAWN_INTERVAL, ForegroundElementGenerator.DEFAULT_MIN_SPAWN_INTERVAL));
            SpawnWallFormation();
        }
    }
}