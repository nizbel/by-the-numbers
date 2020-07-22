using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour {

	/*
	 * LEVEL CONSTANTS
	 */
	public const int MAIN_MENU = 0;
	public const int SHIP_SELECTION_MENU = 1;
	public const int OPTIONS_MENU = 2;

	/* 
	 * MAIN MENU TRANSFORM CONSTANTS
	 */
	public static readonly Vector2 MAIN_MENU_SCALE = new Vector2(180, 300);
	public static readonly Vector3 MAIN_MENU_POSITION = new Vector3(404, -105, 0);
	public static readonly int MENU_TRANSFORMATION_SPEED = 16;

	[SerializeField]
	private Canvas canvas;

	/* 
	 * Buttons for the many states of the menu 
	 */
	[SerializeField]
	private GameObject mainButton;
	[SerializeField]
	private GameObject selectShipButton;
	[SerializeField]
	private GameObject optionsButton;

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
	void Start () {
		mainButton.SetActive(true);
		selectShipButton.SetActive(false);
		optionsButton.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ToggleShipSelection() {
		if (state == MAIN_MENU) {
			mainButton.SetActive(false);
		} else {
			selectShipButton.SetActive(false);
		}
		this.GetComponent<ShipSelectScreenMainMenu>().enabled = true;
	}

	public void ToggleOptions() {
		if (state == MAIN_MENU) {
			mainButton.SetActive(false);
		} else {
			optionsButton.SetActive(false);
		}
		this.GetComponent<OptionsMainMenu>().enabled = true;
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
		mainButton.SetActive(false);
		selectShipButton.SetActive(false);
		optionsButton.SetActive(false);

		// See which buttons have to be activated 
		switch (state) {
		case MAIN_MENU:
			mainButton.SetActive(true);
			break;

		case SHIP_SELECTION_MENU:
			selectShipButton.SetActive(true);
			break;

		case OPTIONS_MENU:
			optionsButton.SetActive(true);
			break;
		}
	}

	public Canvas GetCanvas() {
		return canvas;
	}
}
