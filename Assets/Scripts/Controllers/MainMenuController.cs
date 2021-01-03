using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class MainMenuController : MonoBehaviour {

	/*
	 * LEVEL CONSTANTS
	 */
	public const int MAIN_MENU = 0;
	public const int MODE_SELECTION_MENU = 1;
	public const int OPTIONS_MENU = 2;
	public const int HIGH_SCORE_MENU = 3;

	/* 
	 * MAIN MENU TRANSFORM CONSTANTS
	 */
	public static readonly Vector2 MAIN_MENU_SCALE = new Vector2(180, 300);
	public static readonly Vector3 MAIN_MENU_POSITION = new Vector3(404, -105, 0);
	public static readonly int MENU_TRANSFORMATION_SPEED = 16;

	[SerializeField]
	private Canvas canvas = null;

	/* 
	 * Buttons for the many states of the menu 
	 */
	[SerializeField]
	private GameObject mainButtons = null;
	[SerializeField]
	private GameObject selectModeButtons = null;
	[SerializeField]
	private GameObject optionsButtons = null;
	[SerializeField]
	private GameObject highScoreButtons = null;

	public static MainMenuController controller;

	/*
	 * Maps the current state of the MENU
	 */
	public int state = 0;

	void Awake() {
		if (controller == null) {
			controller = this;
		}
		else {
			Destroy(gameObject);
		}
	}
	// Use this for initialization
	void Start() {
		ShowButtons(mainButtons);
		HideButtons(selectModeButtons);
		HideButtons(optionsButtons);
		HideButtons(highScoreButtons);
	}


	public void ToggleModeSelection() {
		if (state == MAIN_MENU) {
			HideButtons(mainButtons);
		}
		else {
			HideButtons(selectModeButtons);
		}
		this.GetComponent<ModeSelectScreenMainMenu>().enabled = true;
	}

	public void ToggleOptions() {
		if (state == MAIN_MENU) {
			HideButtons(mainButtons);
		}
		else {
			HideButtons(optionsButtons);
		}
		this.GetComponent<OptionsMainMenu>().enabled = true;
	}

	public void ToggleHighScore() {
		if (state == MAIN_MENU) {
			HideButtons(mainButtons);
        } else {
			HideButtons(highScoreButtons);
        }
		this.GetComponent<HighScoreMainMenu>().enabled = true;
    }

	public void QuitGame() {
		Application.Quit();
    }

	/*
	 * Getters and Setters
	 */
	public int GetState() {
		return this.state;
	}

	public void SetState(int state) {
		// Deactivate current state buttons
		//HideButtons(mainButtons);
		//HideButtons(selectModeButtons);
		//HideButtons(optionsButtons);
		switch (this.state) {
			case MAIN_MENU:
				HideButtons(mainButtons);
				break;

			case MODE_SELECTION_MENU:
				HideButtons(selectModeButtons);
				break;

			case OPTIONS_MENU:
				HideButtons(optionsButtons);
				break;

			case HIGH_SCORE_MENU:
				HideButtons(highScoreButtons);
				break;
		}

		this.state = state;

		// See which buttons have to be activated 
		switch (state) {
			case MAIN_MENU:
				ShowButtons(mainButtons);
				break;

			case MODE_SELECTION_MENU:
				ShowButtons(selectModeButtons);
				break;

			case OPTIONS_MENU:
				ShowButtons(optionsButtons);
				break;

			case HIGH_SCORE_MENU:
				ShowButtons(highScoreButtons);
				break;
		}
	}

	private void HideButtons(GameObject buttonsGroup) {
		foreach (Transform child in buttonsGroup.transform) {
			Array.ForEach(child.gameObject.GetComponentsInChildren<Button>(), x => x.enabled = false);
			Array.ForEach(child.gameObject.GetComponentsInChildren<Toggle>(), x => x.enabled = false);
			Array.ForEach(child.gameObject.GetComponentsInChildren<Image>(), x => x.enabled = false);
			Array.ForEach(child.gameObject.GetComponentsInChildren<Text>(), x => x.enabled = false);
		}
	}
	private void ShowButtons(GameObject buttonsGroup) {
		foreach (Transform child in buttonsGroup.transform) {
			Array.ForEach(child.gameObject.GetComponentsInChildren<Button>(), x => x.enabled = true);
			Array.ForEach(child.gameObject.GetComponentsInChildren<Toggle>(), x => x.enabled = true);
			Array.ForEach(child.gameObject.GetComponentsInChildren<Image>(), x => x.enabled = true);
			Array.ForEach(child.gameObject.GetComponentsInChildren<Text>(), x => x.enabled = true);
		}
	}

	public Canvas GetCanvas() {
		return canvas;
	}
}
