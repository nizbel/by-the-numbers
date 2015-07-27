using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class StageController : MonoBehaviour {

	// Constants
	public const float GHOST_DATA_GATHER_INTERVAL = 0.1f;

	int score = 0;

	int obstaclesPast = 0;

	int blocksCaught = 0;

	int rangeChangersPast = 0;

	// Player data
	Transform player;
	PlayerBlock playerBlockScript;

	// Score text object during stage
	TextMesh scoreText;

	// For range changer cration
	public GameObject rangeChangerPrefab;

	float lastRangeChangerSpawned;

	public static StageController controller;

	void Awake() {
		if (controller == null) {
			controller = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		// Get player objects
		player = GameObject.FindGameObjectWithTag("Player").transform;
		playerBlockScript = player.gameObject.GetComponent<PlayerBlock>();

		// Get score object
		scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<TextMesh>();

		// Keep track for range changer spawning
		lastRangeChangerSpawned = Time.timeSinceLevelLoad;
	}
	
	// Update is called once per frame
	void Update () {
		if ((playerBlockScript.getValue() < ValueRange.rangeController.getMinValue()) ||
		    (playerBlockScript.getValue() > ValueRange.rangeController.getMaxValue())) {
			// Game Over
//			gameOver();
		}
		if (Time.timeSinceLevelLoad - lastRangeChangerSpawned > 10) {
			GameObject newRangeChanger = (GameObject) Instantiate(rangeChangerPrefab, new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x + 2, 0, 0),
			                                                      transform.rotation);
			lastRangeChangerSpawned = Time.timeSinceLevelLoad;
		}

		// Update score text
		scoreText.text = score.ToString();
	}

	// Method for game over
	public void gameOver() {
		// Writes the final position data
		player.GetComponent<GhostBlockDataGenerator>().writeToFile();
		player.GetComponent<GhostBlockDataGenerator>().endFile();

		// Get ghost's data reader component
		GameObject.Find("Ghost Block").GetComponent<GhostBlockDataReader>().closeReader();

		File.Delete("pdata.txt");
		File.Copy("pdataw.txt", "pdata.txt");

		// Calls game controller for state change
		GameController.controller.changeState(0);
	}

	// Method when player hits a block
	public void blockCaught() {
		blocksCaught++;
		score++;
	}

	// Player passed though range changer
	public void pastThroughRangeChanger() {
		rangeChangersPast++;
		score++;
	}

	/*
	 * Getters and setters
	 */

	public int getScore() {
		return score;
	}

	public int getBlocksCaught() {
		return blocksCaught;
	}

	public void setBlocksCaught(int blocksCaught) {
		this.blocksCaught = blocksCaught;
	}

	public int getRangeChangersPast() {
		return rangeChangersPast;
	}
	
	public void setRangeChangersPast(int rangeChangersPast) {
		this.rangeChangersPast = rangeChangersPast;
	}


}
