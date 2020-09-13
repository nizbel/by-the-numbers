using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptionsMainMenu : MonoBehaviour {
	
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
		if (mainMenu.GetState() == MainMenuController.MAIN_MENU) {
			float screenSize = Camera.main.orthographicSize * Screen.width / Screen.height;
			
			this.transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime*MainMenuController.MENU_TRANSFORMATION_SPEED);
			this.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(this.GetComponent<RectTransform>().sizeDelta, new Vector2(
				Camera.main.WorldToScreenPoint(new Vector3(screenSize*0.9f, 0, 0)).x / mainMenu.GetCanvas().scaleFactor, Screen.height*0.9f / mainMenu.GetCanvas().scaleFactor), Time.deltaTime*MainMenuController.MENU_TRANSFORMATION_SPEED);
			
			if (Mathf.Abs(this.transform.localPosition.magnitude) < 0.5f 
			    && Mathf.Abs(this.GetComponent<RectTransform>().sizeDelta.magnitude - new Vector2(
				Camera.main.WorldToScreenPoint(new Vector3(screenSize*0.9f, 0, 0)).x / mainMenu.GetCanvas().scaleFactor, Screen.height*0.9f / mainMenu.GetCanvas().scaleFactor).magnitude) < 0.5f) {
				
				this.transform.localPosition = Vector3.zero;
				this.GetComponent<RectTransform>().sizeDelta = new Vector2(
					Camera.main.WorldToScreenPoint(new Vector3(screenSize*0.9f, 0, 0)).x / mainMenu.GetCanvas().scaleFactor, Screen.height*0.9f / mainMenu.GetCanvas().scaleFactor);
				mainMenu.SetState(MainMenuController.OPTIONS_MENU);
				this.enabled = false;
			}
		} else {
			this.transform.localPosition = Vector3.Lerp(transform.localPosition, MainMenuController.MAIN_MENU_POSITION, Time.deltaTime*MainMenuController.MENU_TRANSFORMATION_SPEED);
			this.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(this.GetComponent<RectTransform>().sizeDelta, MainMenuController.MAIN_MENU_SCALE, Time.deltaTime*MainMenuController.MENU_TRANSFORMATION_SPEED);
			
			if (Mathf.Abs(this.transform.localPosition.magnitude - MainMenuController.MAIN_MENU_POSITION.magnitude) < 0.5f
			    && Mathf.Abs(this.GetComponent<RectTransform>().sizeDelta.magnitude - MainMenuController.MAIN_MENU_SCALE.magnitude) < 0.5f) {
				
				this.transform.localPosition = MainMenuController.MAIN_MENU_POSITION;
				this.GetComponent<RectTransform>().sizeDelta = MainMenuController.MAIN_MENU_SCALE;
				mainMenu.SetState(MainMenuController.MAIN_MENU);
				this.enabled = false;
			}
		}
	}
}
