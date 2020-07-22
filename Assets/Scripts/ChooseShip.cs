using UnityEngine;
using System.Collections;

public class ChooseShip : MonoBehaviour {

	[SerializeField]
	private GameObject[] spaceshipPrefabs;

	private GameObject currentShip;

	void Awake() {
		GameObject ship = null;
		//TODO FIX THIS WITH THE STORYBOARD CUTSCENE
		if (GameController.controller.getShipType() > 0) {
			ship = (GameObject) Instantiate(spaceshipPrefabs[GameController.controller.getShipType()], this.transform.position, this.transform.localRotation);
		} else { 
			// Default ship
			ship = (GameObject)Instantiate(spaceshipPrefabs[0], this.transform.position, this.transform.localRotation);
		}
		ship.transform.parent = this.transform;
		currentShip = ship;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
