using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private const float VERTICAL_SPEED = 2.5f;
	private const float MAX_TURNING_ANGLE = 0.05f;
	private const float TURNING_SPEED = 8.5f;
	private const float STABILITY_TURNING_POSITION = 0.33f;

	// Available speed constants
	public const float DEFAULT_SHIP_SPEED = 9.5f;
	public const float ASSIST_MODE_SHIP_SPEED = 7f;

	// In case of game over
	private const float BURNING_SPEED = 2.5f;

	// TODO remove serializefield
	[SerializeField]
	int value = 0;

	float speed = DEFAULT_SHIP_SPEED;

	float targetPosition = 0;

    // Energies in the energy gauge
    GameObject positiveEnergy = null;
    GameObject negativeEnergy = null;

	// Burning material
	[SerializeField]
	Material burningMaterial = null;

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
		if (StageController.controller.GetState() == StageController.GAME_OVER_STATE) {
			float dissolveAmount = Mathf.Lerp(GetComponent<SpriteRenderer>().material.GetFloat("_DissolveAmount"), 1, BURNING_SPEED * Time.deltaTime) ;
			GetComponent<SpriteRenderer>().material.SetFloat("_DissolveAmount", dissolveAmount);
		}
	}

	void FixedUpdate() {
		// Keep value for calculations
		float positionDifference = targetPosition - transform.position.y;

		if (Mathf.Abs(positionDifference) > STABILITY_TURNING_POSITION || (transform.rotation.z == 0 && positionDifference != 0)) {
			transform.rotation = new Quaternion(0, 0, Mathf.Lerp(transform.rotation.z, 
				Mathf.Clamp(positionDifference, -MAX_TURNING_ANGLE, MAX_TURNING_ANGLE), TURNING_SPEED * Time.deltaTime), 1);
        } else if (transform.rotation.z != 0) {
            //float sign = Mathf.Sign(transform.rotation.z);
            //transform.rotation = new Quaternion(0, 0, Mathf.Lerp(transform.rotation.z, 0.01f * -sign, TURNING_SPEED * Time.deltaTime), 1);
            //transform.rotation = new Quaternion(0, 0, Mathf.Lerp(transform.rotation.z, 0, TURNING_SPEED * Time.deltaTime), 1);
            transform.rotation = new Quaternion(0, 0, Mathf.Lerp(transform.rotation.z, 0, 1 - 0.8f/ STABILITY_TURNING_POSITION * Mathf.Abs(positionDifference)), 1);

            //if (Mathf.Sign(transform.rotation.z) != sign) {
            //             transform.rotation = new Quaternion(0, 0, 0, 1);
            //         }
            //if (Mathf.Abs(transform.rotation.z) <= 0.001f) {
            //	Debug.Log("Set position");
            //	transform.rotation = new Quaternion(0, 0, 0, 1);
            //	transform.position = new Vector3(transform.position.x, targetPosition, 0);

            //}
        }
		if (transform.rotation.z != 0) {
			float sign = Mathf.Sign(positionDifference);
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, targetPosition + 0.05f * sign, 0), VERTICAL_SPEED * Time.deltaTime);
			if (Mathf.Sign(targetPosition - transform.position.y) != sign) {
				transform.position = new Vector3(transform.position.x, targetPosition, 0);
			}
		}
	}

	public void SetTargetPosition(float targetPosition) {
		// Limit block position
		targetPosition = LimitTargetPosition(targetPosition);
		this.targetPosition = targetPosition;
	}

	private float LimitTargetPosition(float targetPosition) {
		float shipSize = GetComponent<SpriteRenderer>().sprite.bounds.extents.y * 1.29116f;
		if (targetPosition + shipSize > GameController.GetCameraYMax()) {
			return GameController.GetCameraYMax() - shipSize;
		} else if (targetPosition - shipSize < GameController.GetCameraYMin()) {
			return GameController.GetCameraYMin() + shipSize;
		}
		return targetPosition;
	}

	public void UpdateEnergyBar() {
		if (value > 0) {
			positiveEnergy.GetComponent<Image>().fillAmount = (float)value / StageController.SHIP_VALUE_LIMIT;
			negativeEnergy.GetComponent<Image>().fillAmount = 0;
		}
		else if (value < 0) {
			positiveEnergy.GetComponent<Image>().fillAmount = 0;
			negativeEnergy.GetComponent<Image>().fillAmount = (float)value / -StageController.SHIP_VALUE_LIMIT;
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
			//UpdateShipValue(collider.gameObject.GetComponent<OperationBlock>());

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
			StageController.controller.DestroyShip();
		}
	}

	private void PlayEffect(GameObject gameObject) {
		if (gameObject.GetComponent<AudioSource>() != null) {
			gameObject.GetComponent<AudioSource>().Play();
		}
	}

	private void UpdateShipValue(OperationBlock operationBlock) {
		value = operationBlock.Operation(value);

		// Change color
		UpdateShipColor();

		// Check narrator
		if ((value == ValueRange.rangeController.GetMinValue()) ||
			(value == ValueRange.rangeController.GetMaxValue())) {
			NarratorController.controller.WarnRange();
		}

		UpdateEnergyBar();
	}

	public void UpdateShipValue(int value) {
		this.value += value;

		// Change color
		UpdateShipColor();

		// Check narrator
		if ((value == ValueRange.rangeController.GetMinValue()) ||
			(value == ValueRange.rangeController.GetMaxValue())) {
			NarratorController.controller.WarnRange();
		}

		UpdateEnergyBar();
	}

	private void UpdateShipColor() {
        GetComponent<SpriteRenderer>().color = new Color(1 - Mathf.Max(0, (float)value / StageController.SHIP_VALUE_LIMIT),
            1 - Mathf.Abs((float)value / StageController.SHIP_VALUE_LIMIT), 1 - Mathf.Max(0, (float)value / -StageController.SHIP_VALUE_LIMIT));
    }

	private void TurnOffLight() {
		transform.Find("Point Light 2D").gameObject.SetActive(false);
    }
	private void EngineStop() {
		transform.Find("Engine").gameObject.SetActive(false);
	}

	// Activated on game over
	public void CrashAndBurn() {
		TurnOffLight();
		EngineStop();
		GetComponent<SpriteRenderer>().material = burningMaterial;
		GetComponent<SpriteRenderer>().material.SetFloat("_DissolveAmount", Random.Range(0.4f, 0.6f));
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
