using UnityEngine;
using System.Collections;

public class StageController : MonoBehaviour {

	int score = 0;

	int obstaclesPast = 0;

	Transform player;
	PlayerBlock playerBlockScript;

	GameObject scoreText;

	public GameObject rangeChangerPrefab;

	float lastRangeChangerSpawned;

	public static StageController controller;

	void Awake() {
		controller = this;
	}

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		playerBlockScript = player.gameObject.GetComponent<PlayerBlock>();
//		scoreText = GameObject.FindGameObjectWithTag("Score");
		lastRangeChangerSpawned = Time.timeSinceLevelLoad;
	}
	
	// Update is called once per frame
	void Update () {
		if ((playerBlockScript.value < ValueRange.rangeController.getMinValue()) ||
		    (playerBlockScript.value > ValueRange.rangeController.getMaxValue())) {
			// Game Over
			GameController.controller.changeState(0);
		}
		if (Time.timeSinceLevelLoad - lastRangeChangerSpawned > 10) {
			GameObject newRangeChanger = (GameObject) Instantiate(rangeChangerPrefab, new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x + 2, 0, 0),
			                                                      transform.rotation);
			lastRangeChangerSpawned = Time.timeSinceLevelLoad;
		}
	}

	public int getScore() {
		return score;
	}


}
