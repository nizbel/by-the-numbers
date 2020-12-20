﻿using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	private const float VERTICAL_SPEED = 2.7f;
	private const float MAX_TURNING_ANGLE = 0.05f;
	private const float TURNING_SPEED = 8.5f;
	private const float STABILITY_TURNING_POSITION = 0.33f;
	private const float DEFAULT_X_POSITION = -5.8f;
	private const float BASE_SHOCK_FREQUENCY = 6f;
	private const float ENERGY_SHOCK_FREQUENCY = 15f;

	// Bullet time constants
	private const float DEFAULT_GHOST_TIMER = 0.15f;

	// Available speed constants
	public const float DEFAULT_SHIP_SPEED = 9.2f;
	public const float ASSIST_MODE_SHIP_SPEED = DEFAULT_SHIP_SPEED * 0.8f;

	// In case of game over
	private const float BURNING_SPEED = 2.2f;

	int value = 0;

	float speed = DEFAULT_SHIP_SPEED;

	float targetPosition = 0;

	SpriteRenderer spaceShipSpriteRenderer = null;

    // Energies in the energy gauge
    GameObject positiveEnergy = null;
    GameObject negativeEnergy = null;
	// Energy shock animation
	GameObject energyShock = null;

	SpriteRenderer burningAnimationSpriteRenderer = null;

	/*
	 * Bullet time
	 */
	BulletTimeActivator bulletTimeScript = null;
	float duration = 1.25f;
	bool bulletTimeActive = false;
	[SerializeField]
	GameObject ghostEffect = null;
	float ghostTimer = 0;

	// TODO Test
	int pitchCounter = 0;
	public const float DEFAULT_PITCH_TIMER = 0.5f;
	float pitchTimer = 0;

	public static PlayerController controller;

	void Awake() {
		if (controller == null) {
			controller = this;
			positiveEnergy = GameObject.Find("Positive Energy Bar");
			negativeEnergy = GameObject.Find("Negative Energy Bar");
			energyShock = transform.Find("Energy Shock").gameObject;

			burningAnimationSpriteRenderer = transform.Find("Burning Animation").GetComponent<SpriteRenderer>();

			spaceShipSpriteRenderer = transform.Find("Spaceship").GetComponent<SpriteRenderer>();
		}
		else {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		energyShock.SetActive(false);

		// Get bullet time
		bulletTimeScript = transform.Find("Bullet Time Detection").GetComponent<BulletTimeActivator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (StageController.controller.GetState() == StageController.GAME_OVER_STATE) {
			float dissolveAmount = Mathf.Lerp(burningAnimationSpriteRenderer.material.GetFloat("_DissolveAmount"), 1, BURNING_SPEED * Time.deltaTime) ;
			burningAnimationSpriteRenderer.material.SetFloat("_DissolveAmount", dissolveAmount);

			burningAnimationSpriteRenderer.transform.localScale = Vector3.Lerp(burningAnimationSpriteRenderer.transform.localScale, burningAnimationSpriteRenderer.transform.localScale * 4.5f, Time.deltaTime);
		} else if (!StageController.controller.GetGamePaused()) {
			if (pitchTimer > 0) {
				pitchTimer -= Time.deltaTime;
				if (pitchTimer <= 0) {
					pitchCounter = 0;
                }
            }

			if (bulletTimeActive) {
				duration -= Time.unscaledDeltaTime;
				if (duration <= 0) {
					DeactivateBulletTime();
				}
            }
        }
	}

	void FixedUpdate() {
		if (StageController.controller.GetState() != StageController.GAME_OVER_STATE && !StageController.controller.GetGamePaused()) {
			// Keep value for calculations
			float positionDifference = targetPosition - transform.position.y;

			// Check if should turn to move
			if (Mathf.Abs(positionDifference) > STABILITY_TURNING_POSITION || (transform.rotation.z == 0 && positionDifference != 0)) {
				transform.rotation = new Quaternion(0, 0, Mathf.Lerp(transform.rotation.z,
					Mathf.Clamp(positionDifference, -MAX_TURNING_ANGLE, MAX_TURNING_ANGLE), TURNING_SPEED * Time.unscaledDeltaTime), 1);
				
				// Set bullet time activator to moving
				bulletTimeScript.SetMoving(true);
			}
			// If is moving, check if reached position to get back to 0 rotation
			else if (transform.rotation.z != 0) {
				transform.rotation = new Quaternion(0, 0, Mathf.Lerp(transform.rotation.z, 0, 1 - 0.8f / STABILITY_TURNING_POSITION * Mathf.Abs(positionDifference)), 1);
			}
			// If turned to move, move
			if (transform.rotation.z != 0) {
				float sign = Mathf.Sign(positionDifference);
				transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, targetPosition + 0.05f * sign, 0), VERTICAL_SPEED * Time.unscaledDeltaTime);
				if (Mathf.Sign(targetPosition - transform.position.y) != sign) {
					transform.position = new Vector3(transform.position.x, targetPosition, 0);

					// Set bullet time activator to static
					bulletTimeScript.SetMoving(false);
				}
			}

			if (bulletTimeActive) {
				ghostTimer -= Time.unscaledDeltaTime;
				if (ghostTimer <= 0 && positionDifference != 0) {
					ghostTimer = DEFAULT_GHOST_TIMER;
					GameObject ghost = GameObject.Instantiate(ghostEffect);
					ghost.transform.position = transform.position + Vector3.left * speed * Time.unscaledDeltaTime;
                }
            }
		}
	}

	public void SetTargetPosition(float targetPosition) {
		// Limit ship position
		targetPosition = LimitTargetPosition(targetPosition);
		this.targetPosition = targetPosition;
	}

	private float LimitTargetPosition(float targetPosition) {
		float shipSize = spaceShipSpriteRenderer.sprite.bounds.extents.y;
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
		float maxValue = ValueRange.controller.GetMaxValue();
		float minValue = maxValue - 2 * ValueRange.INTERVAL;

		// TODO Add for positive and separate methods
		// Check if should use shock animation
		if (maxValue - value <= 2) {
			energyShock.SetActive(true);

			// TODO Remove max workaround to avoid division by 0
			int difference = (int)Mathf.Max(maxValue - value, 0);

			// Change base of the shock
			ParticleSystem.EmissionModule emission = energyShock.transform.Find("Base").GetComponent<ParticleSystem>().emission;
			emission.rateOverTimeMultiplier = BASE_SHOCK_FREQUENCY / (difference + 1);

			// TODO make this better, perhaps separating into a different script only for shock
			// Change energy of the shock
			energyShock.transform.Find("Negative Energy").gameObject.SetActive(false);
			GameObject positiveEnergyShock = energyShock.transform.Find("Positive Energy").gameObject;
			positiveEnergyShock.SetActive(true);
			emission = positiveEnergyShock.GetComponent<ParticleSystem>().emission;
			emission.rateOverTimeMultiplier = ENERGY_SHOCK_FREQUENCY / (difference + 1);

			// Change disintegrating parts
			ParticleSystem partsSystem = energyShock.transform.Find("Disintegrating parts").GetComponent<ParticleSystem>();
			ParticleSystem.MainModule partsMainSystem = partsSystem.main;
			partsMainSystem.startColor = spaceShipSpriteRenderer.color;
			emission = partsSystem.emission;
			emission.rateOverTimeMultiplier = 3f / (difference + 1);
		} else if (value - minValue <= 2) {
			energyShock.SetActive(true);

			// TODO Remove max workaround to avoid division by 0
			int difference = (int)Mathf.Max(value - minValue,0);
			
			// Change base of the shock
			ParticleSystem.EmissionModule emission = energyShock.transform.Find("Base").GetComponent<ParticleSystem>().emission;
			emission.rateOverTimeMultiplier = BASE_SHOCK_FREQUENCY / (difference + 1);

			// Change energy of the shock
			energyShock.transform.Find("Positive Energy").gameObject.SetActive(false);
			GameObject negativeEnergyShock = energyShock.transform.Find("Negative Energy").gameObject;
			negativeEnergyShock.SetActive(true);
			emission = negativeEnergyShock.GetComponent<ParticleSystem>().emission;
			emission.rateOverTimeMultiplier = ENERGY_SHOCK_FREQUENCY / (difference + 1);

			// Change disintegrating parts
			ParticleSystem partsSystem = energyShock.transform.Find("Disintegrating parts").GetComponent<ParticleSystem>();
			ParticleSystem.MainModule partsMainSystem = partsSystem.main;
			partsMainSystem.startColor = spaceShipSpriteRenderer.color;
			emission = partsSystem.emission;
			emission.rateOverTimeMultiplier = 3f / (difference + 1);

		} else if (energyShock.activeSelf) {
			energyShock.SetActive(false);
		}

        GameObject barMask = GameObject.Find("Energy Bar Mask");
		barMask.transform.localPosition = new Vector3((maxValue - 5) * barMask.GetComponent<RectTransform>().rect.width / 30, barMask.transform.localPosition.y, barMask.transform.localPosition.z);
		foreach (Transform childTransform in barMask.transform) {
			childTransform.localPosition = new Vector3(-barMask.transform.localPosition.x, 0, 0);
		}
	}

	public void EnergyCollisionReaction(Collider2D collider) {
		// Play sound on collision
		PlayEffect(collider.gameObject);

		// Update pitch data
		pitchTimer = DEFAULT_PITCH_TIMER;
		pitchCounter += 1;

		// Disappear energy
		collider.gameObject.GetComponent<Energy>().Disappear();
		StageController.controller.EnergyCaught();
	}

	public void PowerUpCollisionReaction(Collider2D collider) {
		PowerUpController.controller.SetEffect(collider.gameObject.GetComponent<PowerUp>().getType());

		// Play sound on collision
		PlayEffect(collider.gameObject);

		//TODO disappear power up
		collider.gameObject.GetComponent<SpriteRenderer>().enabled = false;
		collider.gameObject.GetComponent<BoxCollider2D>().enabled = false;

	}

	// TODO Organize this
	ContactPoint2D explosionContactPoint;
	public void ObstacleCollisionReaction(ContactPoint2D contactPoint) {
		explosionContactPoint = contactPoint;
        StageController.controller.GetCurrentForegroundLayer().SetPlayerSpeed(0);
        StageController.controller.DestroyShip();
	}

	private void PlayEffect(GameObject gameObject) {
		if (gameObject.GetComponent<AudioSource>() != null) {
			gameObject.GetComponent<AudioSource>().pitch = 1 + (value * 0.25f/15) + 0.05f * pitchCounter;
			gameObject.GetComponent<AudioSource>().Play();
		}
	}

	public void UpdateShipValue(int value) {
		// Don't do it if game is already over
		if (StageController.controller.GetState() != StageController.GAME_OVER_STATE) {
			this.value += value;

			// Change colors
			UpdateShipColor();

			UpdateEnergyBar();

			// Check narrator
			if ((this.value == ValueRange.controller.GetMinValue()) ||
				(this.value == ValueRange.controller.GetMaxValue())) {
				NarratorController.controller.WarnRange();
			}
			// Check for game Over
			else if (this.value < ValueRange.controller.GetMinValue()) {
				// Make burning animation negative influenced
				burningAnimationSpriteRenderer.material.SetColor("_DissolveColorOuter", new Color(4.5f, 0, 0, 1));
				burningAnimationSpriteRenderer.material.SetColor("_DissolveColorMiddle", new Color(3.5f, 1f, 0, 1));
				burningAnimationSpriteRenderer.material.SetColor("_DissolveColorInner", new Color(2.5f, 2f, 0, 1));
				StageController.controller.DestroyShip();
			}
			else if (this.value > ValueRange.controller.GetMaxValue()) {
				// Make burning animation positive influenced
				burningAnimationSpriteRenderer.material.SetColor("_DissolveColorOuter", new Color(0, 2.5f, 4.5f, 1));
				burningAnimationSpriteRenderer.material.SetColor("_DissolveColorMiddle", new Color(0, 2f, 3.5f, 1));
				burningAnimationSpriteRenderer.material.SetColor("_DissolveColorInner", new Color(0, 1.5f, 2.5f, 1));
				StageController.controller.DestroyShip();
			}
		}
    }

	private void UpdateShipColor() {
		spaceShipSpriteRenderer.color = new Color(1 - Mathf.Max(0, (float)value / StageController.SHIP_VALUE_LIMIT),
            1 - Mathf.Abs((float)value / StageController.SHIP_VALUE_LIMIT), 1 - Mathf.Max(0, (float)value / -StageController.SHIP_VALUE_LIMIT));

		// Get root value to show color strongly on the initial steps
		float rootValue = Mathf.Sqrt(Mathf.Abs((float)value) / StageController.SHIP_VALUE_LIMIT) * 0.5f;
		Color engineFireColor = new Color(Mathf.Max(0, 0.5f - rootValue * Mathf.Sign(value)),
						Mathf.Max(0, 0.5f - rootValue),
						Mathf.Max(0, 0.5f + rootValue * Mathf.Sign(value)));

		// Update engines burst color
		foreach (Transform engineParticle in transform.transform.Find("Engine")) {
			// TODO Check if energy blast should have a different color setting

			// Change color over lifetime
			ParticleSystem.ColorOverLifetimeModule colorOverLifetime = engineParticle.GetComponent<ParticleSystem>().colorOverLifetime;

			Gradient newGradient = colorOverLifetime.color.gradient;
			GradientColorKey[] colorKeys = newGradient.colorKeys;
			for (int i = 0; i < colorKeys.Length; i++) {
				if (colorKeys[i].color != Color.white) {
					colorKeys[i].color = engineFireColor;

				}
			}

			newGradient.SetKeys(colorKeys, newGradient.alphaKeys);

			colorOverLifetime.color = newGradient;
        }
	}

	private void TurnOffLight() {
		transform.Find("Point Light 2D").gameObject.SetActive(false);
    }
	private void DestroyEngine() {
		foreach (Transform engineParticle in transform.transform.Find("Engine")) { 
			ParticleSystem particleSystem = engineParticle.GetComponent<ParticleSystem>();
			if (particleSystem.isPlaying) {
				particleSystem.Stop();
			} else { 
				particleSystem.Play();

				if (explosionContactPoint.collider != null) {
					// TODO Find a better way to insert this info
					// Set explosion starting at contact position
					engineParticle.transform.position = new Vector3(explosionContactPoint.point.x, explosionContactPoint.point.y, engineParticle.transform.position.z);

					// Move particles away from the explosion
					ParticleSystem.ForceOverLifetimeModule forceModule = particleSystem.forceOverLifetime;
					forceModule.enabled = true;
					forceModule.space = ParticleSystemSimulationSpace.World;

					ParticleSystem.MinMaxCurve curveX = forceModule.x;
					curveX.constant = 100 * (transform.position.x - explosionContactPoint.point.x);
					forceModule.x = curveX;
					ParticleSystem.MinMaxCurve curveY = forceModule.y;
					curveY.constant = 100 * (transform.position.y - explosionContactPoint.point.y);
					forceModule.y = curveY;
				}
			}
		}
	}

	private void StopEnergyShock() {
		foreach (Transform child in energyShock.transform) {
			child.GetComponent<ParticleSystem>().Stop();
        }
    }

	// Activated on game over
	public void CrashAndBurn() {
		TurnOffLight();
		DestroyEngine();
		StopEnergyShock();

		// Disable sprite renderer to use burning animation
		spaceShipSpriteRenderer.enabled = false;

		// Disable ghost effect and bullet time
		DeactivateBulletTime();

		// Rotate to give impression of bits going through different directions each time
		burningAnimationSpriteRenderer.gameObject.SetActive(true);
		burningAnimationSpriteRenderer.material.SetFloat("_DissolveAmount", Random.Range(0.4f, 0.6f));
		burningAnimationSpriteRenderer.transform.Rotate(0, 0, Random.Range(0, 360));

		// Deactivate collider and rigidbody
		// TODO Test if this is sufficient
		spaceShipSpriteRenderer.gameObject.GetComponent<BoxCollider2D>().enabled = false;
		spaceShipSpriteRenderer.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;

        // Slightly shake camera
        GameController.GetCamera().GetComponent<CameraShake>().Shake(0.05f, 0.5f);

        // Disable input controller
        InputController.controller.enabled = false;

		// Disable pause button
		GameObject.Find("Pause Button").GetComponent<Button>().interactable = false;
	}

	/*
	 * Bullet time stuff
	 */
	public void ActivateBulletTime() {
		TimeController.controller.SetTimeScale(0.2f);
		bulletTimeActive = true;
		MusicController.controller.SetMusicVolumeInGame(0.4f);
    }

	public void DeactivateBulletTime(bool changeTimeScale=true) {
		bulletTimeActive = false;
		MusicController.controller.SetMusicVolumeInGame(1);

		// Return time to normal
		TimeController.controller.SetTimeScale(1);
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

	public GameObject GetSpaceship() {
		return spaceShipSpriteRenderer.gameObject;
	}

	public SpriteRenderer GetSpaceshipSpriteRenderer() {
		return spaceShipSpriteRenderer;
    }

	public SpriteRenderer GetBurningAnimationSpriteRenderer() {
		return burningAnimationSpriteRenderer;

	}
}
