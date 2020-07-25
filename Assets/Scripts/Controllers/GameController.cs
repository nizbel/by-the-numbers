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
	public const int GAMEPLAY = 1;
	public const int GAME_OVER = 2;

	ScoreData scoreData;

	public bool gameStarted = false;

	public bool gamePaused = false;

	// Determines which ship is the player using
	private int shipType;

	public static GameController controller;

	/*
	 * Maps the current state of the game
	 */
	public int state = 0;
	
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

	public int getState() {
		return state;
	}

	public void changeState(int newState) {
		state = newState;
		switch(newState) {
		case MAIN_MENU:
			gameStarted = false;
			SceneManager.LoadScene("Menu");
			break;
		case GAMEPLAY:
			gameStarted = true;
			SceneManager.LoadScene("Game");
			break;
		case GAME_OVER:
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

	public ScoreData GetScoreData() {
		return scoreData;
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

	/*
	 * Getters and Setters
	 */
	public int GetShipType() {
		return shipType;
	}
	
	public void SetShipType(int shipType) {
		this.shipType = shipType;
	}

}
