using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour {

	PlayerController playerController;

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
		playerController = GameObject.FindGameObjectWithTag("Player Controller").GetComponent<PlayerController>();
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
			playerController.SetBlockPosition(hitPosition.y);
		} else if (Input.GetKeyDown("space")) {
			Debug.Log("Skipped current stage");
			StageController.controller.SkipCurrentEvent();
		}
	}
}
