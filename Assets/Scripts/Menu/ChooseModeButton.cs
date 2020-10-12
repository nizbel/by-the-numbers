using UnityEngine;
using System.Collections;

public class ChooseModeButton : MonoBehaviour {

	public const int STORY_MODE = 1;
	public const int INFINITE_MODE = 2;

	[SerializeField]
	private int mode;

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	public void SelectMode() {
		//GameController.controller.SetShipType(this.shipType);
		if (mode == STORY_MODE) {
			// Start on day one
			GameController.controller.SetCurrentDay(1);
			GameController.controller.ChangeState(GameController.GAMEPLAY_STORY);
		} else {
			GameController.controller.ChangeState(GameController.GAMEPLAY_INFINITE);
		}
	}

	/*
	 * Getters and Setters
	 */
	public int GetMode() {
		return mode;
	}
}
