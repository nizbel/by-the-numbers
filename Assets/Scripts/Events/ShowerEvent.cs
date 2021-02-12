using UnityEngine;

public class ShowerEvent : ForegroundEvent {
	public GameObject[] obstacleGeneratorPrefabList;

	ElementsEnum elementType;

    MeteorGenerator.Duration showerDuration;

    MeteorGenerator.Intensity intensity;

	public void SetElementType(ElementsEnum elementType) {
        this.elementType = elementType;
    }
    public void SetShowerDuration(MeteorGenerator.Duration showerDuration) {
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

		// Add duration to generator
		TimedDurationObject durationScript = newGenerator.gameObject.AddComponent<TimedDurationObject>();
		durationScript.Duration = 5;
		durationScript.WaitTime = 1.2f;
		durationScript.Activate();
		// Activate generator after wait time
		newGenerator.enabled = false;
		durationScript.AddOnWaitListener(newGenerator.Enable);

		// Play warning on panel
		StageController.controller.PanelWarnDanger();

		// Disappear
		Destroy(gameObject);
    }
}
