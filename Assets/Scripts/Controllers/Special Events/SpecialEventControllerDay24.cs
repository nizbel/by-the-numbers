using UnityEngine;
using System.Collections;

public class SpecialEventControllerDay24: MonoBehaviour {
    float duration;

    /*
	 * Energy wave prefab
	 */
    [SerializeField]
    GameObject energyWavePrefab;
    Coroutine waveGeneration = null;

    // Use this for initialization
    void Start() {
        // Fill duration
        duration = StageController.controller.GetCurrentMomentDuration();

        waveGeneration = StartCoroutine(SpawnWaves());
    }

    // Update is called once per frame
    void FixedUpdate() {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) {
            StopCoroutine(waveGeneration);
            Destroy(gameObject);
        }
    }

    void SpawnSmallWave(float waveDuration, float centerPosition) {
        // Spawn wave composed by random energies
        EnergyWaveGeneration waveGenerator = GameObject.Instantiate(energyWavePrefab).GetComponent<EnergyWaveGeneration>();
        waveGenerator.SetAvailableElements(new ElementsEnum[] { ElementsEnum.POSITIVE_ENERGY, ElementsEnum.NEGATIVE_ENERGY });
        waveGenerator.SetDuration(waveDuration);
        waveGenerator.SetFrequency(Random.Range(ElementWavesEvent.HIGH_FREQUENCY, ElementWavesEvent.HIGH_FREQUENCY * 2));
        waveGenerator.SetAmplitude(GameController.GetCameraYMax() / 4);
        waveGenerator.SetCenterPositionY(centerPosition);
        waveGenerator.SetStartingAngle(Random.Range(0, 360f));
    }

    IEnumerator SpawnWaves() {
        float centerPosition = Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax());
        float screenSize = GameController.GetCameraYMax() - GameController.GetCameraYMin();
        while (true) {
            float waveDuration = DefineWaveDuration();

            SpawnSmallWave(waveDuration, centerPosition);
            centerPosition += Random.Range(screenSize * 0.25f, screenSize * 0.75f);
            // If position surpasses screen upper border, return it through the lower border
            if (centerPosition > GameController.GetCameraYMax()) {
                centerPosition -= screenSize;
            }
            yield return new WaitForSeconds(Random.Range(3f, 6f));
        }
    }

    float DefineWaveDuration() {
        return Mathf.Min(duration, Random.Range(5f, 8f));
    }

}