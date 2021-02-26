using UnityEngine;

public class ShowerEvent : ForegroundEvent {
	public const float SHOWER_WARNING_PERIOD = 1.2f;

	// Durations
	// TODO Change amount to duration
	private const float MIN_SHORT_AMOUNT = 4.5f;
	private const float MAX_SHORT_AMOUNT = 6.5f;
	private const float MIN_LONG_AMOUNT = 8.5f;
	private const float MAX_LONG_AMOUNT = 10.5f;

	public enum Duration {
		Short,
		Long
	}

	public GameObject[] obstacleGeneratorPrefabList;

	ElementsEnum elementType;

    Duration showerDuration;

    MeteorGenerator.Intensity intensity;

	public void SetElementType(ElementsEnum elementType) {
        this.elementType = elementType;
    }
    public void SetShowerDuration(Duration showerDuration) {
        this.showerDuration = showerDuration;
    }
    public void SetIntensity(MeteorGenerator.Intensity intensity) {
        this.intensity = intensity;
    }

    protected override void StartEvent() {
		// Choose one of available generators
		int chosenIndex = Random.Range(0, obstacleGeneratorPrefabList.Length);

		// Define a radial position from the middle horizontal right
		float angle = Random.Range(-0.25f, 0.25f) * Mathf.PI;
		float x = GameController.GetCameraXMax() * 1.25f + Mathf.Cos(angle) * GameController.GetCameraYMax();
		float y = Mathf.Sin(angle) * GameController.GetCameraYMax();
		Vector3 spawnPosition = new Vector3(x, y, 0);

		MeteorGenerator newGenerator = GameObject.Instantiate(obstacleGeneratorPrefabList[chosenIndex],
			spawnPosition, Quaternion.identity).GetComponent<MeteorGenerator>();

		// Set generator parameters
		newGenerator.SetIntensity(intensity);
		newGenerator.SetElementType(elementType);

		// Add duration to generator
		TimedDurationObject durationScript = newGenerator.gameObject.AddComponent<TimedDurationObject>();
		durationScript.Duration = DefineDuration();
		durationScript.WaitTime = SHOWER_WARNING_PERIOD;
		durationScript.Activate();
		// Activate generator after wait time
		newGenerator.enabled = false;
		durationScript.AddOnWaitListener(newGenerator.Enable);

		// Play warning on panel
		StageController.controller.PanelWarnDanger();

		// Disappear
		Destroy(gameObject);
    }

	private float DefineDuration() {
		switch (showerDuration) {
			case Duration.Short:
				return Random.Range(MIN_SHORT_AMOUNT, MAX_SHORT_AMOUNT);

			case Duration.Long:
				return Random.Range(MIN_LONG_AMOUNT, MAX_LONG_AMOUNT);

			default:
				return 0;
        }
	}
}
