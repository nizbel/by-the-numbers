using UnityEngine;
using UnityEngine.UI;
using System;

public class GameMenuController : MonoBehaviour {

	/*
	 * LEVEL CONSTANTS
	 */
	public const int GAME_MENU = 0;
	public const int OPTIONS_MENU = 1;

	/* 
	 * MAIN MENU TRANSFORM CONSTANTS
	 */
	public static readonly Vector2 GAME_MENU_SCALE = new Vector2(180, 300);
	public static readonly Vector3 GAME_MENU_POSITION = Vector3.zero;

	[SerializeField]
	private Canvas canvas;

	/* 
	 * Buttons for the many states of the menu 
	 */
	[SerializeField]
	private GameObject mainButton;
	[SerializeField]
	private GameObject optionsButton;

	public static GameMenuController controller;
	
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
	void Start () {
        ShowButtons(mainButton);
		HideButtons(optionsButton);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void ToggleOptions() {
		if (state == GAME_MENU) {
			HideButtons(mainButton);
		} else {
			HideButtons(optionsButton);
		}
		this.GetComponent<OptionsGameMenu>().enabled = true;
	}

	/*
	 * Getters and Setters
	 */
	public int GetState() {
		return this.state;
	}

	public void SetState(int state) {
		this.state = state;
		// Deactivate all buttons
		HideButtons(mainButton);
		HideButtons(optionsButton);

		// See which buttons have to be activated 
		switch (state) {
		case GAME_MENU:
			ShowButtons(mainButton);
			break;

		case OPTIONS_MENU:
			ShowButtons(optionsButton);
			break;
		}
	}

	private void HideButtons(GameObject buttonsGroup) {
		foreach (Transform child in buttonsGroup.transform) {
			Array.ForEach(child.gameObject.GetComponentsInChildren<Button>(), x => x.interactable = false);
			Array.ForEach(child.gameObject.GetComponentsInChildren<Toggle>(), x => x.interactable = false);
			Array.ForEach(child.gameObject.GetComponentsInChildren<Image>(), x => x.enabled = false);
			Array.ForEach(child.gameObject.GetComponentsInChildren<Text>(), x => x.enabled = false);
        }
    }
	private void ShowButtons(GameObject buttonsGroup) {
		foreach (Transform child in buttonsGroup.transform) {
			Array.ForEach(child.gameObject.GetComponentsInChildren<Button>(), x => x.interactable = true);
			Array.ForEach(child.gameObject.GetComponentsInChildren<Toggle>(), x => x.interactable = true);
			Array.ForEach(child.gameObject.GetComponentsInChildren<Image>(), x => x.enabled = true);
			Array.ForEach(child.gameObject.GetComponentsInChildren<Text>(), x => x.enabled = true);
		}
	}

	public Canvas GetCanvas() {
		return canvas;
	}
}
