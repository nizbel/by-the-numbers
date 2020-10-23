﻿using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour {

	private const float MAX_SPEED_ARROW = 6.5f;
	private const float ACCELERATION = 4.5f;

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
		else if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.UpArrow)) {
			speed = 0;
		}
		else if (Input.GetKeyDown("space")) {
			if (GameController.controller.GetState() == GameController.GAMEPLAY_STORY 
				&& GameController.GetGameInfo().StagePlayed(GameController.controller.GetCurrentDay())) {
				StageController.controller.SkipCutscenes(); 
			}
		} 
		// TODO Remove this for production
		else if (Input.GetKeyDown(KeyCode.S)) {
			Debug.Log("Skipped current stage");
			StageController.controller.SkipCurrentEvent();
		}


		if (Input.GetKey(KeyCode.DownArrow)) {
			speed -= ACCELERATION * Time.deltaTime;
        }
		if (Input.GetKey(KeyCode.UpArrow)) {
			speed += ACCELERATION * Time.deltaTime;
		}
		if (speed != 0) {
			speed = Mathf.Clamp(speed, -MAX_SPEED_ARROW, MAX_SPEED_ARROW);
            PlayerController.controller.SetTargetPosition(PlayerController.controller.transform.position.y + speed);
        }
	}
}
