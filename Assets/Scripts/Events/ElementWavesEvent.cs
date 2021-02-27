using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ElementWavesEvent : ForegroundEvent {
	public const float SHOWER_WARNING_PERIOD = 1.2f;

	// Durations
	private const float SHORT_DURATION = 5f;
	private const float LONG_DURATION = 10f;

    // Frequencies
    public const float LOW_FREQUENCY = 0.25f;
    public const float MEDIUM_FREQUENCY = 0.5f;
    public const float HIGH_FREQUENCY = 0.75f;

    // Starting angles
    private const float BOTTOM_ANGLE = 270f;
    private const float CENTER_ANGLE = 0;
    private const float TOP_ANGLE = 90f;

    public enum DurationEnum {
		Short,
		Long
    }

    public enum AmplitudeEnum {
        HalfScreen,
        QuarterScreen,
        ThirdScreen,
        SixthScreen
    }

    public enum CenterPositionYEnum {
        ScreenCenter,
        RandomTopHalf,
        RandomBottomHalf,
        UpperHalf,
        LowerHalf
    }

    public enum StartingAngleEnum {
        Random,
        Bottom,
        Center,
        Top
    }

    public enum FrequencyEnum {
        Random,
        Low, 
        Medium,
        High
    }


    [Serializable]
    public class ElementsWaveData {
        public ElementsEnum[] availableElements;

        public EnergyWaveGeneration.WaveTypeEnum[] availableWaveTypes;

        public EnergyWaveGeneration.GenerationIntervalEnum generationInterval;

        public CenterPositionYEnum[] centerPositionsY;

        public AmplitudeEnum amplitude;

        public StartingAngleEnum startingAngle;

        public FrequencyEnum frequency;

        public DurationEnum duration;

        public float startDelay;
    }

    ElementsWaveData[] elementWaves;

    /*
	 * Energy wave prefab
	 */
    [SerializeField]
    GameObject energyWavePrefab;

    public void SetElementWaves(ElementsWaveData[] elementWaves) {
        this.elementWaves = elementWaves;
    }

    protected override void StartEvent() {
		foreach (ElementsWaveData elementsWave in elementWaves) {
            StartCoroutine(GenerateWave(elementsWave));
        }

        // Disappear
        Destroy(gameObject);
    }

    IEnumerator GenerateWave(ElementsWaveData elementsWave) {
		yield return new WaitForSeconds(elementsWave.startDelay);
        EnergyWaveGeneration newWave = GameObject.Instantiate(energyWavePrefab).GetComponent<EnergyWaveGeneration>();
        newWave.SetType(elementsWave.availableWaveTypes[Random.Range(0, elementsWave.availableWaveTypes.Length)]);
        newWave.SetAvailableElements(elementsWave.availableElements);
        newWave.SetDuration(DefineDuration(elementsWave.duration));
        newWave.SetFrequency(DefineFrequency(elementsWave.frequency));
        newWave.SetAmplitude(DefineAmplitude(elementsWave.amplitude));
        newWave.SetCenterPositionY(DefineCenterPositionY(elementsWave.centerPositionsY[Random.Range(0, elementsWave.centerPositionsY.Length)]));
        newWave.SetStartingAngle(DefineStartingAngle(elementsWave.startingAngle));
    }

    private float DefineDuration(DurationEnum duration) {
		switch (duration) {
			case DurationEnum.Short:
				return SHORT_DURATION;

			case DurationEnum.Long:
				return LONG_DURATION;

			default:
				return 0;
        }
	}

    private float DefineFrequency(FrequencyEnum frequency) {
        switch (frequency) {
            case FrequencyEnum.Low:
                return LOW_FREQUENCY;

            case FrequencyEnum.Medium:
                return MEDIUM_FREQUENCY;

            case FrequencyEnum.High:
                return HIGH_FREQUENCY;

            case FrequencyEnum.Random:
                return Random.Range(LOW_FREQUENCY, HIGH_FREQUENCY);

            default:
                return 0;
        }
    }

    private float DefineAmplitude(AmplitudeEnum amplitude) {
        switch (amplitude) {
            case AmplitudeEnum.HalfScreen:
                return GameController.GetCameraYMax();

            case AmplitudeEnum.QuarterScreen:
                return GameController.GetCameraYMax()/2;

            case AmplitudeEnum.ThirdScreen:
                return GameController.GetCameraYMax()*3/2;

            case AmplitudeEnum.SixthScreen:
                return GameController.GetCameraYMax()/3;

            default:
                return 0;
        }
    }
    private float DefineCenterPositionY(CenterPositionYEnum centerPositionY) {
        switch (centerPositionY) {
            case CenterPositionYEnum.ScreenCenter:
                return 0;

            case CenterPositionYEnum.RandomTopHalf:
                return Random.Range(0.5f, GameController.GetCameraYMax() - 0.5f);

            case CenterPositionYEnum.RandomBottomHalf:
                return Random.Range(GameController.GetCameraYMin() + 0.5f, -0.5f);

            case CenterPositionYEnum.UpperHalf:
                return GameController.GetCameraYMax()/2;

            case CenterPositionYEnum.LowerHalf:
                return GameController.GetCameraYMin()/2;

            default:
                return 0;
        }
    }

    private float DefineStartingAngle(StartingAngleEnum startingAngle) {
        switch (startingAngle) {
            case StartingAngleEnum.Bottom:
                return BOTTOM_ANGLE;

            case StartingAngleEnum.Center:
                return CENTER_ANGLE;

            case StartingAngleEnum.Top:
                return TOP_ANGLE;

            case StartingAngleEnum.Random:
                return Random.Range(TOP_ANGLE, BOTTOM_ANGLE);

            default:
                return 0;
        }
    }

}
