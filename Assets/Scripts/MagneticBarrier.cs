using UnityEngine;
using UnityEngine.Events;

public class MagneticBarrier : MonoBehaviour {

	Transform player;

	bool finished = false;

	bool positive;

	protected UnityEvent onPast = new UnityEvent();

	// Use this for initialization
	void Start () {
		player = PlayerController.controller.transform;
	}
	
	// Update is called once per frame
	void Update () {
		// TODO Use a collider to apply shake and past through actions
		if (!finished) {
			if (player.position.x > this.transform.position.x) {
				ValueRange.controller.ChangeRange(positive);
				StageController.controller.PastThroughMagneticBarrier();

				// Call events registered
				onPast.Invoke();

                finished = true;
				PlayerController.controller.UpdateEnergyBar();
			}
		}
	}

	public void AddPastListener(UnityAction action) {
		onPast.AddListener(action);
	}

	/*
	 * Getters and Setters
	 */
	public void SetPositive(bool positive) {
		this.positive = positive;
		if (this.positive) {
			GetComponent<SpriteRenderer>().color = new Color(0.05f, 0.05f, 0.92f);
		}
		else {
			GetComponent<SpriteRenderer>().color = new Color(0.92f, 0.05f, 0.05f);
		}
	}
}
