using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour {

	private const float ACCELERATION = 0.28f;

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
	
	// Update is called once per frame
	void Update () {
		if ((Input.GetMouseButtonDown(0)) || (Input.touchCount > 0)) {
			if (EventSystem.current.IsPointerOverGameObject()) {
				return;
			}
			if (StageController.controller.GetCurrentMomentType() == MomentTypeEnum.Cutscene) {
				return;
            }
			Vector3 hitPosition = Vector3.zero;
			RaycastHit2D[] hits = new RaycastHit2D[1];
			switch (Application.platform) {
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer:
					hits = Physics2D.RaycastAll(GameController.GetCamera().ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
					hitPosition = GameController.GetCamera().ScreenToWorldPoint(Input.mousePosition);
					break;
				case RuntimePlatform.Android:
					hits = Physics2D.RaycastAll(GameController.GetCamera().ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
					hitPosition = GameController.GetCamera().ScreenToWorldPoint(Input.GetTouch(0).position);
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
			NarratorController.controller.StopSpeech();
			StageController.controller.SkipCurrentMoment();
		}// TODO Remove this for production
		else if (Input.GetKeyDown(KeyCode.D)) {
			Debug.Log("Dead");
			StageController.controller.DestroyShip();
		}

		if (StageController.controller.GetCurrentMomentType() != MomentTypeEnum.Cutscene) {
			if (StageController.controller.GetState() != StageController.GAME_OVER_STATE) {
				if (Input.GetAxis("Vertical") == 0) {
					speed = 0;
				} else {
                    speed += Mathf.Sign(Input.GetAxis("Vertical")) * ACCELERATION * Time.unscaledDeltaTime;
				}
			}
			if (speed != 0) {
				PlayerController.controller.SetTargetPosition(PlayerController.controller.GetTargetPosition() + speed);
                //Debug.Log("Target " + PlayerController.controller.GetTargetPosition());
            }
		}
	}
}
