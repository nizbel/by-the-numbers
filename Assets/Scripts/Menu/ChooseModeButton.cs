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

	public void SelectMode() {
		if (mode == STORY_MODE) {
            GameController.controller.SetCurrentDay(CurrentDayController.GetInitialDay());
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
