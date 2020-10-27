using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundEventGenerator : MonoBehaviour
{
	/* 
	 * Constants
	 */
	public const float DEFAULT_MAX_EVENT_SPAWN_CHANCE = 50;

	/*
	 * Energy formation prefabs
	 */
	public List<GameObject> energyFormationList;
	//private bool formationSpawned = false;
	//private float formationCooldown = 0;
	//private float lastEventSpawnTime = 0;

	/*
	 * Obstacle generator prefabs
	 */
	public List<GameObject> obstacleGeneratorPrefabList;

	// Start is called before the first frame update
	void Start()
    {

	}

    // Update is called once per frame
    void Update()
    {
        
    }

    // Spawn
    public void SpawnEvent() {
		// TODO Improve choosing formation
		// Check if spawned will be a formation or obstacle generator
		if (GameController.RollChance(65) || StageController.controller.GetCurrentSpecialCharges() < 3) {
			// Define current spawning position
			float curSpawnPosition = ForegroundElementGenerator.SPAWN_CAMERA_OFFSET + GameController.GetCameraXMax();

			GameObject energyFormation = energyFormationList[Random.Range(0, energyFormationList.Count)];
			//GameObject energyFormation = energyFormationList[2];

			float formationScreenOffset = energyFormation.GetComponent<Formation>().GetScreenOffset();
			(bool, GameObject) spawnedFormation = SpawnForegroundElement(energyFormation,
				new Vector3(curSpawnPosition + formationScreenOffset, Random.Range(-1, 1), 0),
				GameObjectUtil.GenerateRandomRotation());

			// TODO check if spawned to count on stage's special spawning charges
			if (spawnedFormation.Item1) {
				//formationSpawned = true;
				ForegroundController.controller.EventSpawned(spawnedFormation.Item2.GetComponent<ForegroundEvent>());
				//formationCooldown = 0.1f * spawnedFormation.Item2.transform.childCount;
				//lastEventSpawnTime = Time.time;
				//Debug.Log(currentSpecialSpawnChance);
			}
		}
		else {
			// Spawn obstacle generator
			GameObject obstacleGenerator = obstacleGeneratorPrefabList[Random.Range(0, obstacleGeneratorPrefabList.Count)];

			// Define a radial position from the middle horizontal right
			float angle = Random.Range(-0.25f, 0.25f) * Mathf.PI;
			float x = GameController.GetCameraXMax() + Mathf.Cos(angle) * GameController.GetCameraYMax();
			float y = Mathf.Sin(angle) * GameController.GetCameraYMax();
			Vector3 spawnPosition = new Vector3(x, y, 0);

			(bool, GameObject) spawnedGeneration = SpawnForegroundElement(obstacleGenerator,
				spawnPosition, new Quaternion(0, 0, 0, 1), false);

			// TODO check if spawned to count on stage's special spawning charges
			if (spawnedGeneration.Item1) {
				//StageController.controller.UseSpecialCharges(3);
				//lastEventSpawnTime = Time.time;

				// Add duration to generator
				TimedDurationObject durationScript = spawnedGeneration.Item2.AddComponent<TimedDurationObject>();
				durationScript.Duration = 5;
				durationScript.WaitTime = 1.2f;
				// Make meteor generator activate after wait time
				spawnedGeneration.Item2.GetComponent<MeteorGenerator>().enabled = false;
				durationScript.AddOnWaitListener(spawnedGeneration.Item2.GetComponent<MeteorGenerator>().Enable);

				// Play warning on panel
				StageController.controller.PanelWarnDanger();

				//Debug.Log(currentSpecialSpawnChance);
			}
		}
	}

	private (bool, GameObject) SpawnForegroundElement(GameObject foregroundPrefab, Vector3 position, Quaternion rotation,
		bool randomizedX = true) {
		if (randomizedX) {
			// Add randomness to the horizontal axis
			float cameraLengthFraction = (GameController.GetCameraXMax() - GameController.GetCameraXMin()) / 4;
			position = new Vector3(position.x + Random.Range(0, cameraLengthFraction), position.y, position.z);
		}

		// Spawn element
		GameObject newForegroundElement = (GameObject)Instantiate(foregroundPrefab, position, new Quaternion(0, 0, 0, 1));
		newForegroundElement.transform.localRotation = rotation;

		// Check if bound overlap
		if (newForegroundElement.GetComponent<Formation>() != null) {
			// Get children of formation
			foreach (Transform child in newForegroundElement.transform) {
				foreach (GameObject block in GameObject.FindGameObjectsWithTag("Block")) {
					if (block.transform.parent != newForegroundElement.transform) {
						if (child.GetComponent<Collider2D>().bounds.Intersects(block.GetComponent<Collider2D>().bounds)) {
							Destroy(newForegroundElement);
							return (false, null);
						}
					}
				}
			}
		}

		return (true, newForegroundElement);
	}
}
