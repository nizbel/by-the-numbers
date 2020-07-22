using UnityEngine;
using System.Collections;

public class ChooseShip : MonoBehaviour {

	[SerializeField]
	private GameObject[] spaceshipPrefabs;

	void Awake() {
		//TODO FIX THIS WITH THE STORYBOARD CUTSCENE
		if (GameController.controller.getShipType() > 0) {
			GameObject actualShip = transform.GetChild(0).gameObject;
			GameObject ship = (GameObject) Instantiate(spaceshipPrefabs[GameController.controller.getShipType()], this.transform.position, this.transform.localRotation);
			actualShip.GetComponent<SpriteRenderer>().sprite = ship.GetComponent<SpriteRenderer>().sprite;
			Destroy (ship);
		}	
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
