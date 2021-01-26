using UnityEngine;
using UnityEditor;

public class HighScoreMainMenu : MonoBehaviour {

	private const float HIGH_SCORE_MENU_POSITION_X = 0;
	private const float HIGH_SCORE_MENU_POSITION_Y = 0;
	private const float HIGH_SCORE_MENU_WIDTH = 0.9f;
	private const float HIGH_SCORE_MENU_HEIGHT = 0.9f;
	private const float APPROXIMATION_CONSTANT = 0.5f;

	MainMenuController mainMenu;

	void Awake() {
		if (mainMenu == null) {
			this.mainMenu = this.GetComponent<MainMenuController>();
		}
	}


	// Update is called once per frame
	void Update() {
		if (mainMenu.GetState() == MainMenuController.MAIN_MENU) {
			float screenSize = GameController.GetCamera().orthographicSize * Screen.width / Screen.height;
			Vector3 highScoreMenuPosition = new Vector3(HIGH_SCORE_MENU_POSITION_X, HIGH_SCORE_MENU_POSITION_Y, 0);

			this.transform.localPosition = Vector3.Lerp(transform.localPosition, highScoreMenuPosition, Time.deltaTime * MainMenuController.MENU_TRANSFORMATION_SPEED);

			this.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(this.GetComponent<RectTransform>().sizeDelta, new Vector2(
				GameController.GetCamera().WorldToScreenPoint(new Vector3(screenSize * HIGH_SCORE_MENU_WIDTH, 0, 0)).x / mainMenu.GetCanvas().scaleFactor, Screen.height * HIGH_SCORE_MENU_HEIGHT / mainMenu.GetCanvas().scaleFactor), Time.deltaTime * MainMenuController.MENU_TRANSFORMATION_SPEED);

			// If size and position are APPROXIMATION_CONSTANT close, finish animation and set desired position and size
			if (Mathf.Abs(this.transform.localPosition.magnitude - highScoreMenuPosition.magnitude) < APPROXIMATION_CONSTANT
				&& Mathf.Abs(this.GetComponent<RectTransform>().sizeDelta.magnitude - new Vector2(
				GameController.GetCamera().WorldToScreenPoint(new Vector3(screenSize * HIGH_SCORE_MENU_WIDTH, 0, 0)).x / mainMenu.GetCanvas().scaleFactor, Screen.height * HIGH_SCORE_MENU_HEIGHT / mainMenu.GetCanvas().scaleFactor).magnitude) < APPROXIMATION_CONSTANT) {

				this.transform.localPosition = highScoreMenuPosition;
				this.GetComponent<RectTransform>().sizeDelta = new Vector2(
					GameController.GetCamera().WorldToScreenPoint(new Vector3(screenSize * HIGH_SCORE_MENU_WIDTH, 0, 0)).x / mainMenu.GetCanvas().scaleFactor, Screen.height * HIGH_SCORE_MENU_HEIGHT / mainMenu.GetCanvas().scaleFactor);
				mainMenu.SetState(MainMenuController.HIGH_SCORE_MENU);
				this.enabled = false;
			}
		}
		else {
			this.transform.localPosition = Vector3.Lerp(transform.localPosition, MainMenuController.MAIN_MENU_POSITION, Time.deltaTime * MainMenuController.MENU_TRANSFORMATION_SPEED);
			this.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(this.GetComponent<RectTransform>().sizeDelta, MainMenuController.MAIN_MENU_SCALE, Time.deltaTime * MainMenuController.MENU_TRANSFORMATION_SPEED);

			if (Mathf.Abs(this.transform.localPosition.magnitude - MainMenuController.MAIN_MENU_POSITION.magnitude) < APPROXIMATION_CONSTANT
				&& Mathf.Abs(this.GetComponent<RectTransform>().sizeDelta.magnitude - MainMenuController.MAIN_MENU_SCALE.magnitude) < APPROXIMATION_CONSTANT) {

				this.transform.localPosition = MainMenuController.MAIN_MENU_POSITION;
				this.GetComponent<RectTransform>().sizeDelta = MainMenuController.MAIN_MENU_SCALE;
				mainMenu.SetState(MainMenuController.MAIN_MENU);
				this.enabled = false;
			}
		}
	}
}