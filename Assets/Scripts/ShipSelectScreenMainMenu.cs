using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipSelectScreenMainMenu : MonoBehaviour {

	MainMenuController mainMenu;

	void Awake() {
		if (mainMenu == null) {
			this.mainMenu = this.GetComponent<MainMenuController>();
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (mainMenu.getState() == MainMenuController.MAIN_MENU) {
			float screenSize = Camera.main.orthographicSize * Screen.width / Screen.height;
			
			this.transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0,
			                                                                       -200, 0), Time.deltaTime*6);
			this.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(this.GetComponent<RectTransform>().sizeDelta, new Vector2(
				Camera.main.WorldToScreenPoint(new Vector3(screenSize*1.01f, 0, 0)).x / mainMenu.getCanvas().scaleFactor, 180), Time.deltaTime*6);

			if (Mathf.Abs(this.transform.localPosition.magnitude - (new Vector3(0, -200, 0).magnitude)) < 0.5f 
			    && Mathf.Abs(this.GetComponent<RectTransform>().sizeDelta.magnitude - (new Vector2(
				Camera.main.WorldToScreenPoint(new Vector3(screenSize*1.01f, 0, 0)).x / mainMenu.getCanvas().scaleFactor, 180).magnitude)) < 0.5f) {

				this.transform.localPosition = new Vector3(0, -200, 0);
				this.GetComponent<RectTransform>().sizeDelta = new Vector2(
					Camera.main.WorldToScreenPoint(new Vector3(screenSize*1.01f, 0, 0)).x / mainMenu.getCanvas().scaleFactor, 180);
				mainMenu.setState(MainMenuController.SHIP_SELECTION_MENU);
				this.enabled = false;
			}
		} else {
			this.transform.localPosition = Vector3.Lerp(transform.localPosition, MainMenuController.MAIN_MENU_POSITION, Time.deltaTime*6);
			this.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(this.GetComponent<RectTransform>().sizeDelta, MainMenuController.MAIN_MENU_SCALE, Time.deltaTime*6);

			if (Mathf.Abs(this.transform.localPosition.magnitude - MainMenuController.MAIN_MENU_POSITION.magnitude) < 0.5f
			    && Mathf.Abs(this.GetComponent<RectTransform>().sizeDelta.magnitude - MainMenuController.MAIN_MENU_SCALE.magnitude) < 0.5f) {

				this.transform.localPosition = MainMenuController.MAIN_MENU_POSITION;
				this.GetComponent<RectTransform>().sizeDelta = MainMenuController.MAIN_MENU_SCALE;
				mainMenu.setState(MainMenuController.MAIN_MENU);
				this.enabled = false;
			}
		}
	}
}
