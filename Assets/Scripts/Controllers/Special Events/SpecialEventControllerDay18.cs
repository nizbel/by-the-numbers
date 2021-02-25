using UnityEngine;
using System.Collections;

public class SpecialEventControllerDay18 : MonoBehaviour {
    float duration;

    /*
	 * Energy wave prefab
	 */
    [SerializeField]
    GameObject energyWavePrefab;

    // Use this for initialization
    void Start() {
        // Fill duration
        duration = StageController.controller.GetCurrentMomentDuration();

        StartCoroutine(SpawnWaves());
    }

    // Update is called once per frame
    void FixedUpdate() {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) {
            Destroy(gameObject);
        }
    }


    void SpawnSingleWave(float waveDuration) {
        // Spawn wave composed by random energies
        EnergyWaveGeneration waveGenerator = GameObject.Instantiate(energyWavePrefab).GetComponent<EnergyWaveGeneration>();
        waveGenerator.SetAvailableElements(new ElementsEnum[] { ElementsEnum.POSITIVE_ENERGY, ElementsEnum.NEGATIVE_ENERGY });
        waveGenerator.SetDuration(waveDuration);
        waveGenerator.SetFrequency(0.5f);
        waveGenerator.SetAmplitude(GameController.GetCameraYMax() - 1);
    }

    void SpawnDoubleWaves(float waveDuration) {
        // Spawn positive wave
        EnergyWaveGeneration positiveWave = GameObject.Instantiate(energyWavePrefab).GetComponent<EnergyWaveGeneration>();
        positiveWave.SetAvailableElements(new ElementsEnum[] { ElementsEnum.POSITIVE_ENERGY});
        positiveWave.SetDuration(waveDuration);
        positiveWave.SetFrequency(0.75f);
        positiveWave.SetAmplitude(GameController.GetCameraYMax() / 3);
        positiveWave.SetCenterPositionY(GameController.GetCameraYMax()/2);

        // Spawn negative wave
        EnergyWaveGeneration negativeWave = GameObject.Instantiate(energyWavePrefab).GetComponent<EnergyWaveGeneration>();
        negativeWave.SetAvailableElements(new ElementsEnum[] { ElementsEnum.NEGATIVE_ENERGY });
        negativeWave.SetDuration(waveDuration);
        negativeWave.SetFrequency(0.75f);
        negativeWave.SetAmplitude(GameController.GetCameraYMax() / 3);
        negativeWave.SetCenterPositionY(GameController.GetCameraYMin() / 2);
        negativeWave.SetStartingAngle(180f);
    }

    IEnumerator SpawnWaves() {
        float firstWaveDurations = 10f;
        SpawnSingleWave(firstWaveDurations);
        yield return new WaitForSeconds(firstWaveDurations);
        SpawnDoubleWaves(firstWaveDurations);
        yield return new WaitForSeconds(firstWaveDurations);
        SpawnSingleWave(duration);
    }

}