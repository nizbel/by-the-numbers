using UnityEngine;
using System.Collections;

public class BlockSpawner : MonoBehaviour {

	/*
	 * Block prefabs
	 */
	public GameObject addBlockPrefab;
	public GameObject subtractBlockPrefab;
	public GameObject multiplyBlockPrefab;
	public GameObject divideBlockPrefab;

	/*
	 * Power Up prefabs
	 */
	public GameObject neutralizerPrefab;
	public GameObject growthPrefab;

	Transform player;

	float lastSpawn;
	float spawnTimer = 1;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;

		lastSpawn = Time.timeSinceLevelLoad;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad - lastSpawn > spawnTimer) {
			float curSpawnPosition = 3 + Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;

			switch (Random.Range(0, 2)) {
			case 0:
				GameObject newAddBlock = (GameObject) Instantiate(addBlockPrefab, new Vector3(curSpawnPosition, Random.Range(-3.1f, 3.1f), 0), transform.rotation);
				break;

			case 1:
				GameObject newSubtractBlock = (GameObject) Instantiate(subtractBlockPrefab, new Vector3(curSpawnPosition, Random.Range(-3.1f, 3.1f), 0), transform.rotation);
				break;
			}
			lastSpawn = Time.timeSinceLevelLoad;

			// TODO get a better way of spawning power ups
			switch (Random.Range(0, 30)) {
			case 0:
				GameObject neutralizer = (GameObject) Instantiate(neutralizerPrefab, new Vector3(curSpawnPosition, Random.Range(-3.1f, 3.1f), 0), transform.rotation);
				break;

			case 1:
				GameObject growth = (GameObject) Instantiate(growthPrefab, new Vector3(curSpawnPosition, Random.Range(-3.1f, 3.1f), 0), transform.rotation);
				break;
			}

			// Modify spawn timer randomly
			spawnTimer = Random.Range(0.3f, 1.2f);
		}
	}
}
