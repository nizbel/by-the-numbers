using UnityEngine;
using System.Collections;

public class ChooseSpaceshipButton : MonoBehaviour {

	/*
	 * 0 = Stock Ship
	 * 1 = Zeta Ship
	 */

	[SerializeField]
	private int shipType;

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	public void selectShip() {
		GameController.controller.SetShipType(this.shipType);
		// Start on day one
		GameController.controller.SetCurrentDay(1);
		GameController.controller.ChangeState(GameController.GAMEPLAY_STORY);
	}

	/*
	 * Getters and Setters
	 */
	public int getShipType() {
		return shipType;
	}

	public void setShipType(int shipType) {
		this.shipType = shipType;
	}
}
