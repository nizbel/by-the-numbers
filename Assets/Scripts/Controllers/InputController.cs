﻿using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour {

	private const float ACCELERATION = 10.2f;

	private float speed = 0;

	public static InputController controller;

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

	}
	
	// Update is called once per frame
	void Update () {
		if ((Input.GetMouseButtonDown(0)) || (Input.touchCount > 0)) {
			if (EventSystem.current.IsPointerOverGameObject()) {
				return;
			}
			if (StageController.controller.GetCurrentMomentType() == StageMoment.TYPE_CUTSCENE) {
				return;
            }
			Vector3 hitPosition = Vector3.zero;
			RaycastHit2D[] hits = new RaycastHit2D[1];
			switch (Application.platform) {
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer:
					hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
					hitPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					break;
				case RuntimePlatform.Android:
					hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
					hitPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
					break;
			}
			PlayerController.controller.SetTargetPosition(hitPosition.y);
		}
		else if (Input.GetKeyDown("space")) {
			// Allow initial cutscenes to be skipped if stage was played
			if (StageController.controller.GetState() == StageController.STARTING_STATE
				&& GameController.GetGameInfo().StagePlayed(GameController.controller.GetCurrentDay())) {
				StageController.controller.SkipCutscenes(); 
			} 
			// Allow ending cutscenes to be skipped if stage was completed
			else if (StageController.controller.GetState() == StageController.ENDING_STATE
				&& GameController.GetGameInfo().StageDone(GameController.controller.GetCurrentDay())) {
				StageController.controller.SkipCutscenes();
			}
		} 
		// TODO Remove this for production
		else if (Input.GetKeyDown(KeyCode.N)) {
			Debug.Log("Skipped current stage");
			StageController.controller.SkipCurrentMoment();
		}// TODO Remove this for production
		else if (Input.GetKeyDown(KeyCode.D)) {
			Debug.Log("Dead");
			StageController.controller.DestroyShip();
		}

		if (StageController.controller.GetCurrentMomentType() != StageMoment.TYPE_CUTSCENE) {
			if (StageController.controller.GetState() != StageController.GAME_OVER_STATE) {
				if (GetMoveDownKeyUp() || GetMoveUpKeyUp()) {
					speed = 0;
				}
				if (GetMoveDownKey()) {
					speed -= ACCELERATION * Time.deltaTime;
				}
				if (GetMoveUpKey()) {
					speed += ACCELERATION * Time.deltaTime;
				}
			}
			if (speed != 0) {
				PlayerController.controller.SetTargetPosition(PlayerController.controller.transform.position.y + speed);
			}
		}
	}

	bool GetMoveUpKeyUp() {
		return Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W);
	}

	bool GetMoveDownKeyUp() {
		return Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S);
	}
	bool GetMoveUpKey() {
		return Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
	}

	bool GetMoveDownKey() {
		return Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
	}
}
