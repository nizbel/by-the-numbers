using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private const float VERTICAL_SPEED = 2.5f;
	private const float MAX_TURNING_ANGLE = 0.05f;
	private const float TURNING_SPEED = 4.5f;

	public const float DEFAULT_SHIP_SPEED = 9.5f;
	public const float ASSIST_MODE_SHIP_SPEED = 7f;

	[SerializeField]
	int value = 0;

	float speed = DEFAULT_SHIP_SPEED;

	float targetPosition = 0;

    // Energies in the energy gauge
    GameObject positiveEnergy = null;
    GameObject negativeEnergy = null;

	public static PlayerController controller;

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
        positiveEnergy = GameObject.Find("Positive Energy Bar");
        negativeEnergy = GameObject.Find("Negative Energy Bar");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate() {
        if (Mathf.Abs(transform.position.y - targetPosition) > 0.25f) {
			transform.rotation = new Quaternion(0, 0, Mathf.Lerp(transform.rotation.z, Mathf.Clamp(targetPosition - transform.position.y, -MAX_TURNING_ANGLE, MAX_TURNING_ANGLE), TURNING_SPEED * Time.deltaTime), 1);
        } else {
			transform.rotation = new Quaternion(0, 0, Mathf.Lerp(transform.rotation.z, 0, TURNING_SPEED * Time.deltaTime), 1);
        }
		if (transform.rotation.z != 0) {
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, targetPosition, 0), VERTICAL_SPEED * Time.deltaTime);
		}
	}

	public void SetTargetPosition(float targetPosition) {
		// Limit block position
		targetPosition = LimitBlockPosition(targetPosition);
		this.targetPosition = targetPosition;
	}

	private float LimitBlockPosition(float blockPosition) {
		float shipSize = GetComponent<SpriteRenderer>().sprite.bounds.extents.y * transform.localScale.x;
		if (blockPosition + shipSize > GameController.GetCameraYMax()) {
			return GameController.GetCameraYMax() - shipSize;
		} else if (blockPosition - shipSize < GameController.GetCameraYMin()) {
			return GameController.GetCameraYMin() + shipSize;
		}
		return blockPosition;
	}

	public void UpdateEnergyBar() {
		if (value > 0) {
			positiveEnergy.GetComponent<Image>().fillAmount = value / StageController.SHIP_VALUE_LIMIT;
			negativeEnergy.GetComponent<Image>().fillAmount = 0;
		}
		else if (value < 0) {
			positiveEnergy.GetComponent<Image>().fillAmount = 0;
			negativeEnergy.GetComponent<Image>().fillAmount = value / -StageController.SHIP_VALUE_LIMIT;
		}
		else {
			positiveEnergy.GetComponent<Image>().fillAmount = 0;
			negativeEnergy.GetComponent<Image>().fillAmount = 0;
		}

		// TODO Make this better
		float maxValue = ValueRange.rangeController.GetMaxValue();

        GameObject barMask = GameObject.Find("Energy Bar Mask");
		barMask.transform.localPosition = new Vector3((maxValue - 5) * barMask.GetComponent<RectTransform>().rect.width / 30, barMask.transform.localPosition.y, barMask.transform.localPosition.z);
		foreach (Transform childTransform in barMask.transform) {
			childTransform.localPosition = new Vector3(-barMask.transform.localPosition.x, 0, 0);
		}
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag == "Block") {
			UpdateShipValue(collider.gameObject.GetComponent<OperationBlock>());

			// Change player block color
			GetComponent<SpriteRenderer>().color = new Color(1 - Mathf.Max(0, (float)value / StageController.SHIP_VALUE_LIMIT),
				1 - Mathf.Abs((float)value / StageController.SHIP_VALUE_LIMIT), 1 - Mathf.Max(0, (float)value / -StageController.SHIP_VALUE_LIMIT));

			// Play sound on collision
			PlayEffect(collider.gameObject);

			// Disappear block
			collider.gameObject.GetComponent<OperationBlock>().Disappear();
			StageController.controller.BlockCaught();
		}
		else if (collider.gameObject.tag == "Power Up") {
			PowerUpController.controller.SetEffect(collider.gameObject.GetComponent<PowerUp>().getType());

			// Play sound on collision
			PlayEffect(collider.gameObject);

			//TODO disappear power up
			collider.gameObject.GetComponent<SpriteRenderer>().enabled = false;
			collider.gameObject.GetComponent<BoxCollider2D>().enabled = false;
		}
		else if (collider.gameObject.tag == "Obstacle") {
			StageController.controller.GameOver();
		}
	}

	private void PlayEffect(GameObject gameObject) {
		if (gameObject.GetComponent<AudioSource>() != null) {
			gameObject.GetComponent<AudioSource>().Play();
		}
	}

	private void UpdateShipValue(OperationBlock operationBlock) {
		value = operationBlock.Operation(value);

		// Check narrator
		if ((value == ValueRange.rangeController.GetMinValue()) ||
			(value == ValueRange.rangeController.GetMaxValue())) {
			NarratorController.controller.WarnRange();
		}

		UpdateEnergyBar();
	}

	/*
	 * Getters and Setters
	 */

	public int GetValue() {
		return value;
	}

	public void SetValue(int value) {
		this.value = value;
	}

	public float GetSpeed() {
		return speed;
	}

	public void SetSpeed(float speed) {
		this.speed = speed;
	}
}
