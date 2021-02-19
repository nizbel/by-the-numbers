using UnityEngine;
using System.Collections;

public class SpecialEvent2ControllerDay13 : MonoBehaviour {
    /*
     * Speeches
     */
    [SerializeField]
    private Speech curiousFormationsSpeech;

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
        StartCoroutine(PlayNarrator());
        SpawnEnergyWall();

        generation = StartCoroutine(GenerateWalls());
    }

    private void FixedUpdate() {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) {
            StopCoroutine(generation);
            Destroy(gameObject);
        }
    }

    void SpawnEnergyWall() {
        // Spawn walls composed by random energies
        EnergyWall newEnergyWall = GameObject.Instantiate(energyWallPrefab).GetComponent<EnergyWall>();
        newEnergyWall.SetMoving(GameController.RollChance(20));
        newEnergyWall.SetType(EnergyWall.RANDOM_ENERGIES);
    }

    IEnumerator GenerateWalls() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(ForegroundElementGenerator.DEFAULT_MIN_SPAWN_INTERVAL, ForegroundElementGenerator.DEFAULT_MIN_SPAWN_INTERVAL));
            SpawnEnergyWall();
        }
    }

    IEnumerator PlayNarrator() {
        yield return new WaitForSeconds((duration/2) - 1);
        NarratorController.controller.StartMomentSpeech(curiousFormationsSpeech);
    }
}