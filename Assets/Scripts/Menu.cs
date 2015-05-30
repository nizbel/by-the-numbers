using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public GameObject[] buttons;

	// Determine whether the menu is moving, so it shouldn't be clickable
	protected bool movingIn = false;

	protected bool movingOut = false;

	// Determines if the menu is the currently active on screen
	protected bool activeOnScreen = false;

	// Defines the positions menus should be, either getting in or out the screen
	public Vector2 destIn;

	public Vector2 destOut;

	// Maps the current state of the menu
	public int state = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	void Update () {
		if ((!movingIn) && (!movingOut)) {
			if (activeOnScreen) {
				if ((Input.GetMouseButtonDown(0)) || (Input.touchCount > 0)) {
					RaycastHit2D[] hits = new RaycastHit2D[1];
					switch (Application.platform) {
					case RuntimePlatform.WindowsEditor:
					case RuntimePlatform.WindowsPlayer:
						hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
						break;
					case RuntimePlatform.Android:
						hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
						break;
					}
					foreach (RaycastHit2D hitOrig in hits)
					{
						if (hitOrig.collider.tag == "Button") {
							hitOrig.collider.gameObject.GetComponent<Button>().buttonAction();
						}
					}
				}
			}
		}
		else {
			if (movingIn) {
				moveIn();
				if (checkMoveInFinished()) {
					//transform.position = new Vector3(destIn.x, destIn.y, 0);
					movingIn = false;
					activeOnScreen = true;
					Debug.Log(gameObject + " stopped moving in");
				}
			}
			else if (movingOut) {
				moveOut();
				if (checkMoveOutFinished()) {
					//transform.position = new Vector3(destOut.x, destOut.y, 0);
					movingOut = false;
					Debug.Log(gameObject + " stopped moving out");
				}
			}
		}
	}

	public virtual void activateMenu() {

	}

	public virtual void changeState(int newState) {

	}

	public virtual void moveIn() {

	}

	public virtual bool checkMoveInFinished() {
		return false;
	}

	public virtual void moveOut() {

	}

	public virtual bool checkMoveOutFinished() {
		return false;
	}

	public void deactivateMenu() {
		for (int i = 0; i < buttons.Length; i++) {
			buttons[i].SetActive(false);
		}
		gameObject.SetActive(false);
	}
}
