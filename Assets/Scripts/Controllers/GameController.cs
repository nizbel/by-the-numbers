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
	public const int FHD_WINDOWS_RES_X = 1920;
	public const int FHD_WINDOWS_RES_Y = 1080;
	public const int HD_WINDOWS_RES_X = 1366;
	public const int HD_WINDOWS_RES_Y = 768;
	public const int WINDOWS_FHD_RES = 2;
	public const int WINDOWS_HD_RES = 1;

	public const int ANDROID_RES_X = 1136;
	public const int ANDROID_RES_Y = 640;

	GameInfo gameInfo;

	// Keep track of days that have been played in current story run
	int daysPlayed = 0;

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
					(int, int) resolution  = PlayerPrefsUtil.GetResolutionPref();

					Screen.SetResolution(resolution.Item1, resolution.Item2, PlayerPrefsUtil.GetBoolPref(PlayerPrefsUtil.FULL_SCREEN_PREF));

					// Fix mouse if not full screen
					if (!Screen.fullScreen) {
						Cursor.lockState = CursorLockMode.Confined;
					}

                    break;
				case RuntimePlatform.WindowsEditor:
					Screen.SetResolution(FHD_WINDOWS_RES_X, FHD_WINDOWS_RES_Y, PlayerPrefsUtil.GetBoolPref(PlayerPrefsUtil.FULL_SCREEN_PREF));
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
				currentDay = 0;
				SceneManager.LoadScene("Menu");
				break;
			case GAMEPLAY_STORY:
				daysPlayed = 0;
				SceneManager.LoadScene("Story");
				break;
			case GAMEPLAY_INFINITE:
				SceneManager.LoadScene("Infinite");
				break;
			case GAME_OVER_STORY:
				currentDay = 0;
				SceneManager.LoadScene("Game Over");
				TimeController.controller.SetTimeScale(1);
				break;
			case GAME_OVER_INFINITE:
				SceneManager.LoadScene("Game Over");
				TimeController.controller.SetTimeScale(1);
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

	public void UpdateDayInfoSuccess(int score) {
		StageInfo stageInfo = GetGameInfo().GetStageInfoByDay(GetCurrentDay());
		stageInfo.tries++;
		// TODO Test if using assist mode
		stageInfo.wins++;
		stageInfo.UpdateHighScore(score);
		Save();
	}

	public void UpdateDayInfoDefeat() {
		StageInfo stageInfo = GetGameInfo().GetStageInfoByDay(GetCurrentDay());
		stageInfo.tries++;
		Save();
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

	// Full screen control
	public static void SetFullScreen(bool value) {
		Screen.fullScreen = value;

		if (value) {
			Cursor.lockState = CursorLockMode.None;
		}
		else {
			Cursor.lockState = CursorLockMode.Confined;
		}

		PlayerPrefsUtil.SetBoolPref(PlayerPrefsUtil.FULL_SCREEN_PREF, value);
	}

	// Resolution control
	public static void SetResolution(int resOption) {
		switch (resOption) {
			case GameController.WINDOWS_HD_RES:
				Screen.SetResolution(GameController.HD_WINDOWS_RES_X, GameController.HD_WINDOWS_RES_Y, Screen.fullScreen);
				break;

			case GameController.WINDOWS_FHD_RES:
				Screen.SetResolution(GameController.FHD_WINDOWS_RES_X, GameController.FHD_WINDOWS_RES_Y, Screen.fullScreen);
				break;
		}

		PlayerPrefs.SetInt(PlayerPrefsUtil.RESOLUTION_PREF, resOption);
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

	public int GetDaysPlayed() {
		return daysPlayed;
    }

	public void SetDaysPlayed(int daysPlayed) {
		this.daysPlayed = daysPlayed;
    }
}
