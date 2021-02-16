using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptionsGameMenu : MonoBehaviour {
	
	GameMenuController gameMenu;

	void Awake() {
		if (gameMenu == null) {
			this.gameMenu = this.GetComponent<GameMenuController>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (gameMenu.GetState() == GameMenuController.GAME_MENU) {
			float screenSize = GameController.GetCamera().orthographicSize * Screen.width / Screen.height;

			this.transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.unscaledDeltaTime*MainMenuController.MENU_TRANSFORMATION_SPEED);
			this.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(this.GetComponent<RectTransform>().sizeDelta, new Vector2(
				GameController.GetCamera().WorldToScreenPoint(new Vector3(screenSize*0.9f, 0, 0)).x / gameMenu.GetCanvas().scaleFactor, Screen.height*0.9f / gameMenu.GetCanvas().scaleFactor), Time.unscaledDeltaTime * MainMenuController.MENU_TRANSFORMATION_SPEED);

			if (Mathf.Abs(this.transform.localPosition.magnitude) < 0.5f 
			    && Mathf.Abs(this.GetComponent<RectTransform>().sizeDelta.magnitude - new Vector2(
				GameController.GetCamera().WorldToScreenPoint(new Vector3(screenSize*0.9f, 0, 0)).x / gameMenu.GetCanvas().scaleFactor, Screen.height*0.9f / gameMenu.GetCanvas().scaleFactor).magnitude) < 0.5f) {
				
				this.transform.localPosition = Vector3.zero;
				this.GetComponent<RectTransform>().sizeDelta = new Vector2(
					GameController.GetCamera().WorldToScreenPoint(new Vector3(screenSize*0.9f, 0, 0)).x / gameMenu.GetCanvas().scaleFactor, Screen.height*0.9f / gameMenu.GetCanvas().scaleFactor);
				gameMenu.SetState(GameMenuController.OPTIONS_MENU);
				this.enabled = false;
			}
		} else {
			this.transform.localPosition = Vector3.Lerp(transform.localPosition, GameMenuController.GAME_MENU_POSITION, Time.unscaledDeltaTime* MainMenuController.MENU_TRANSFORMATION_SPEED);
			this.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(this.GetComponent<RectTransform>().sizeDelta, GameMenuController.GAME_MENU_SCALE, Time.unscaledDeltaTime * MainMenuController.MENU_TRANSFORMATION_SPEED);
			
			if (Mathf.Abs(this.transform.localPosition.magnitude - GameMenuController.GAME_MENU_POSITION.magnitude) < 0.5f
			    && Mathf.Abs(this.GetComponent<RectTransform>().sizeDelta.magnitude - GameMenuController.GAME_MENU_SCALE.magnitude) < 0.5f) {
				
				this.transform.localPosition = GameMenuController.GAME_MENU_POSITION;
				this.GetComponent<RectTransform>().sizeDelta = GameMenuController.GAME_MENU_SCALE;
				gameMenu.SetState(GameMenuController.GAME_MENU);
				this.enabled = false;
			}
		}
	}
}
