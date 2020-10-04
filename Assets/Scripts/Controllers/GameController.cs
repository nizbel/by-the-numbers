using UnityEngine;
//using System.Collections;
//using System;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameController : MonoBehaviour {

	/*
	 * LEVEL CONSTANTS
	 */
	public const int MAIN_MENU = 0;
	public const int GAMEPLAY_STORY = 1;
	public const int GAMEPLAY_INFINITE = 2;
	public const int GAME_OVER_STORY = 3;
	public const int GAME_OVER_INFINITE = 4;

	ScoreData scoreData;

	public bool gameStarted = false;

	public bool gamePaused = false;

	// Determines which ship is the player using
	private int shipType;

	public static GameController controller;

	/*
	 * Maps the current state of the game
	 */
	int state = 0;

	/*
	 * 
	 */
	
	void Awake() {
		if (controller == null) {
			controller = this;
			switch (Application.platform) {
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.WindowsPlayer:
				Screen.SetResolution(1136, 640, false);
				break;
			case RuntimePlatform.Android:
				Screen.SetResolution(1136, 640, false);
				break;
			}
			DontDestroyOnLoad(gameObject);
			scoreData = new ScoreData();
			Load();
		}
		else {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void ChangeState(int newState) {
		state = newState;
		switch(newState) {
		case MAIN_MENU:
			gameStarted = false;
			SceneManager.LoadScene("Menu");
			break;
		case GAMEPLAY_STORY:
			gameStarted = true;
			SceneManager.LoadScene("Story");
			break;
		case GAMEPLAY_INFINITE:
			gameStarted = true;
			SceneManager.LoadScene("Infinite");
			break;
		case GAME_OVER_STORY:
			gameStarted = false;
			SceneManager.LoadScene("Game Over");
			break;
		case GAME_OVER_INFINITE:
			gameStarted = false;
			SceneManager.LoadScene("Game Over");
			break;
		}
	}

	public void Save() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "ScoreData.lhd");
		
		bf.Serialize(file, scoreData);
		
		file.Close();
	}
	
	public void Load() {
		if (File.Exists(Application.persistentDataPath + "ScoreData.lhd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "ScoreData.lhd", FileMode.Open);
			
			scoreData = (ScoreData) bf.Deserialize(file);
			
			file.Close();
		}
	}


	// Camera bounds
	public static float GetCameraXMax() {
		return Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
	}

	public static float GetCameraXMin() {
		return Camera.main.ScreenToWorldPoint(Vector3.zero).x;
	}

	public static float GetCameraYMax() {
		return Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
	}

	public static float GetCameraYMin() {
		return Camera.main.ScreenToWorldPoint(Vector3.zero).y;
	}

	// Rolls a chance in x% of something happening
	public static bool RollChance(float percentChance) {
		return Random.Range(1f, 100f) <= percentChance;
	}

	/*
	 * Getters and Setters
	 */
	public ScoreData GetScoreData() {
		return scoreData;
	}

	public int GetShipType() {
		return shipType;
	}
	
	public void SetShipType(int shipType) {
		this.shipType = shipType;
	}

	public int GetState() {
		return state;
	}
}
