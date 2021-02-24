using UnityEngine;
using System.Collections;

public class SpecialEventControllerDay18 : MonoBehaviour {
    private const float DEFAULT_SPAWN_OFFSET_X = -2;
    private const float DEFAULT_SPAWN_OFFSET_Y = 10;
    private const float DEFAULT_ELEMENTS_DISTANCE = 2f;
    private const float WAIT_TIME_FOR_WALL = 1f;

    float duration;

    /*
	 * Energy wall prefab
	 */
    [SerializeField]
    GameObject energyWavePrefab;

    EnergyWaveGeneration waveGenerator = null;

    // Use this for initialization
    void Start() {
        // Fill duration
        duration = StageController.controller.GetCurrentMomentDuration();

        // Generates the wall that will pass through the stage moment
        SpawnWaveGenerator();
    }

    // Update is called once per frame
    void FixedUpdate() {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) {
            Destroy(waveGenerator.gameObject);
            Destroy(gameObject);
        }
    }


    void SpawnWaveGenerator() {
        // Spawn walls composed by random energies
        waveGenerator = GameObject.Instantiate(energyWavePrefab).GetComponent<EnergyWaveGeneration>();
        waveGenerator.SetAvailableElements(new ElementsEnum[] { ElementsEnum.POSITIVE_ENERGY, ElementsEnum.NEGATIVE_ENERGY });
        waveGenerator.SetDuration(duration);
        waveGenerator.SetFrequency(4);
        waveGenerator.SetAmplitude(GameController.GetCameraYMax() - 1);
    }

}