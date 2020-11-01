using UnityEngine;
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

	/* 
	 * Res constants
	 */
	public const int WINDOWS_RES_X = 1920;
	public const int WINDOWS_RES_Y = 1080;

	public const int ANDROID_RES_X = 1136;
	public const int ANDROID_RES_Y = 640;

	GameInfo gameInfo;

	bool gameStarted = false;

	public static GameController controller;

	/*
	 * Maps the current state of the game
	 */
	// TODO remove serializing
	[SerializeField]
	int state = 0;

	/*
	 * Keeps current day info in story mode
	 */
	// TODO remove serializing
	[SerializeField]
	int currentDay = 0;

	void Awake() {
		if (controller == null) {
			controller = this;

			// Resolution
			switch (Application.platform) {
				case RuntimePlatform.WindowsPlayer:
					Screen.SetResolution(WINDOWS_RES_X, WINDOWS_RES_Y, true);

					// Fix mouse
					if (!Screen.fullScreen) {
						Cursor.lockState = CursorLockMode.Confined;
					}

                    break;
				case RuntimePlatform.WindowsEditor:
					Screen.SetResolution(WINDOWS_RES_X, WINDOWS_RES_Y, false);
					break;
				case RuntimePlatform.Android:
					Screen.SetResolution(ANDROID_RES_X, ANDROID_RES_Y, true);
					break;
			}

			DontDestroyOnLoad(gameObject);
			gameInfo = new GameInfo();
			Load();
		}
		else {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}


	public void ChangeState(int newState) {
		state = newState;
		switch (newState) {
			case MAIN_MENU:
				gameStarted = false;
				currentDay = 0;
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
				currentDay = 0;
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
		FileStream file = File.Create(Application.persistentDataPath + "/GameInfo.save");

		bf.Serialize(file, gameInfo);

		file.Close();
	}

	public void Load() {
		if (File.Exists(Application.persistentDataPath + "/GameInfo.save")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/GameInfo.save", FileMode.Open);

			gameInfo = (GameInfo)bf.Deserialize(file);

			file.Close();

			//foreach (StageInfo info in gameInfo.listStageInfo) {
			//	Debug.Log(info.day + "..." + info.highScore + "..." + info.played);
			//}
        }
	}

	void OnApplicationQuit() {
		PlayerPrefs.Save();
    }

	public static GameInfo GetGameInfo() {
		return controller.gameInfo;
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
		return Random.Range(0, 100.0f) <= percentChance;
	}

	/*
	 * Getters and Setters
	 */
	public int GetState() {
		return state;
	}

	public int GetCurrentDay() {
		return currentDay;
	}

	public void SetCurrentDay(int currentDay) {
		this.currentDay = currentDay;
	}
}
