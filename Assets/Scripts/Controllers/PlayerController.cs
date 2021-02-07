using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	private const float VERTICAL_SPEED = 8.8f;
	private const float MAX_TURNING_ANGLE = 0.07f;
	private const float TURNING_SPEED = 0.65f;
	private const float DEFAULT_X_POSITION = -5.8f;
	private const float BASE_SHOCK_FREQUENCY = 6f;
	private const float ENERGY_SHOCK_FREQUENCY = 15f;
	private const float SHIP_EXPLOSION_RADIUS = 1.5f;

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

	[SerializeField]
	SpriteRenderer spaceShipSpriteRenderer = null;

	// Energy shock animation
	[SerializeField]
	GameObject energyShock = null;
	// Energy force field
	[SerializeField]
	ParticleSystemForceField energyForceField;

	[SerializeField]
	ParticleSystemRenderer spaceshipDebrisParticles = null;
	[SerializeField]
	SpriteRenderer burningAnimationSpriteRenderer = null;

	/*
	 * Bullet time
	 */
	BulletTimeActivator bulletTimeScript = null;
	float duration = 1.25f;
	bool bulletTimeActive = false;
	[SerializeField]
	[Tooltip("Ghost effect prefab")]
	GameObject ghostEffect = null;
	float ghostTimer = 0;

	[SerializeField]
	ParticleSystem[] speedParticles;

	/*
	 * Ship's light
	 */
	[SerializeField]
	[Tooltip("Ship object with the lighting")]
	Light2D shipLightObject;

	// Energy Bar
	[Header("Energy Bar")]
	[SerializeField]
	[Tooltip("Energy Bar object in the UI")]
	GameObject energyBar;
	// Energies in the energy bar
	GameObject positiveEnergy = null;
	GameObject negativeEnergy = null;
	List<EnergyBarInfluence> energyBarInfluences = new List<EnergyBarInfluence>();

	// TODO Test
	int pitchCounter = 0;
	public const float DEFAULT_PITCH_TIMER = 0.5f;
	float pitchTimer = 0;

	public static PlayerController controller;

	void Awake() {
		if (controller == null) {
			controller = this;
			positiveEnergy = energyBar.transform.Find("Positive Energy Bar").gameObject;
			negativeEnergy = energyBar.transform.Find("Negative Energy Bar").gameObject;

			// Set random seed for ship debris particles
			spaceshipDebrisParticles.material.SetVector("Seed", new Vector4(Random.Range(0, 1f), Random.Range(0, 1f), 0,0));
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

		// TODO Test for speed particles
		UpdateSpeedParticles();
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
			if (Mathf.Abs(positionDifference) > 0.06f) {
				// Set bullet time activator to moving
				bulletTimeScript.SetMoving(true);

				// Change rotation to point to direction
				transform.rotation = new Quaternion(0, 0, Mathf.MoveTowards(transform.rotation.z,
					Mathf.Clamp(positionDifference, -MAX_TURNING_ANGLE, MAX_TURNING_ANGLE), TURNING_SPEED * Time.fixedUnscaledDeltaTime), 1);

				// If turned to move to the right direction, move
				if (positionDifference * transform.rotation.z > 0) {
					transform.position = Vector2.MoveTowards(transform.position, new Vector2(DEFAULT_X_POSITION, targetPosition), VERTICAL_SPEED * Time.fixedUnscaledDeltaTime);

					if (transform.position.y == targetPosition) {
						bulletTimeScript.SetMoving(false);
					}
				}

			}
			else {
				if (transform.rotation.z != 0) {
					// Set bullet time activator to moving
					bulletTimeScript.SetMoving(true);

					transform.rotation = new Quaternion(0, 0, Mathf.MoveTowards(transform.rotation.z, 0,
						TURNING_SPEED * Time.fixedUnscaledDeltaTime), 1);
				}
				// TODO Check if shake can be used
				//else {
				//	float shake = 0.01f;
				//	transform.position = Vector2.MoveTowards(transform.position,
				//		new Vector2(DEFAULT_X_POSITION, targetPosition + Random.Range(-shake, shake)),
				//		verticalSpeed * Time.fixedUnscaledDeltaTime); 
				//}
			}


			// Generate ghost effect during bullet time
			if (bulletTimeActive) {
				ghostTimer -= Time.fixedUnscaledDeltaTime;
				if (ghostTimer <= 0 && positionDifference != 0) {
					ghostTimer = DEFAULT_GHOST_TIMER;
					GameObject ghost = GameObject.Instantiate(ghostEffect);
					ghost.transform.position = transform.position + Vector3.left * speed * Time.fixedUnscaledDeltaTime;
				}
			}

			// TODO Test update speed particles
			if (positionDifference != 0) {
                UpdateSpeedParticles();
            }
        }
		
	}

    private void UpdateSpeedParticles() {
        float cameraSizeY = (GameController.GetCameraYMax() - GameController.GetCameraYMin());
        foreach (ParticleSystem ps in speedParticles) {
            float distanceToShip = Mathf.Abs(ps.transform.position.y - transform.position.y);

            // Set velocity scale
            if (bulletTimeActive) {
                ps.GetComponent<ParticleSystemRenderer>().velocityScale = Mathf.Lerp(0.1f, 3, Mathf.Pow(distanceToShip / cameraSizeY, 2)) / BULLET_TIME_SPEED_PARTICLE_SCALE;
            }
            else {
                ps.GetComponent<ParticleSystemRenderer>().velocityScale = Mathf.Lerp(0.1f, 3, Mathf.Pow(distanceToShip / cameraSizeY, 2));
            }

            // Set emission rate
            ParticleSystem.EmissionModule emissionModule = ps.emission;
            emissionModule.rateOverTime = Mathf.Max(0.1f, 2 * Mathf.Pow((1f - distanceToShip / cameraSizeY), 2));

            // Set amount of particles
            ParticleSystem.MainModule mainModule = ps.main;
            mainModule.maxParticles = Mathf.Max(1, Mathf.RoundToInt(8 * Mathf.Pow((1f - distanceToShip / cameraSizeY), 2)));
        }
    }

    public void SetTargetPosition(float targetPosition) {
		// Limit ship position
		this.targetPosition = LimitTargetPosition(targetPosition);
	}

	private float LimitTargetPosition(float targetPosition) {
		float shipSize = spaceShipSpriteRenderer.sprite.bounds.extents.y;

        return Mathf.Clamp(targetPosition, GameController.GetCameraYMin() + shipSize, GameController.GetCameraYMax() - shipSize);
    }

	// Add energy bar changing animation
	private void InfluenceEnergyBar(int value) {
		//Debug.Log(value);
		// Clear influence list after null values
		while (energyBarInfluences.Count > 0 && energyBarInfluences[0] == null) {
			energyBarInfluences.RemoveAt(0);
        }

		// Check if last influence in list is of the same value
		if (energyBarInfluences.Count > 0 && energyBarInfluences[energyBarInfluences.Count - 1] != null) {
			// Get last influence value position
			EnergyBarInfluence lastInfluence = energyBarInfluences[energyBarInfluences.Count - 1];

			bool lastInfluencePositive = (lastInfluence.GetValuePosition() > 0) == lastInfluence.GetIsIntensifying();
			//Debug.Log((lastInfluence.GetValuePosition() > 0) + "..." + lastInfluence.GetIsIntensifying());
			// Check if last influence's value is the same as current value change
			if (lastInfluencePositive == (value > 0)) {
				//Debug.Log("INFLUENCIA energia igual");
				int valuePosition = lastInfluence.GetValuePosition() + value;
				if (valuePosition == 0) {
					valuePosition += value;
					// If direction changed, should intensify
					AddEnergyBarInfluence(valuePosition, true);
				} else {
					AddEnergyBarInfluence(valuePosition, lastInfluence.GetIsIntensifying());
				}
			} else {
				//Debug.Log("INFLUENCIA energia diferente");
				AddEnergyBarInfluence(lastInfluence.GetValuePosition(), !lastInfluence.GetIsIntensifying());
			}
		} else {
			// The base is ship's current value
			if (this.value * value > 0) {
				//Debug.Log("NORMAL energia igual");
				// Ship is energized with the same value of energy
				AddEnergyBarInfluence(this.value + value, true);
			}
			else if (this.value * value < 0) {
				//Debug.Log("NORMAL energia diferente");
				// Ship is energized with a value contrary to energy
				AddEnergyBarInfluence(this.value, false);
			}
			else {
				//Debug.Log("NORMAL neutro");
				// Ship is neutral
				AddEnergyBarInfluence(value, true);
			}
		}
    }

	private void AddEnergyBarInfluence(int valuePosition, bool isIntensifying) {
        //Debug.Log("VALUE POSITION: " + valuePosition);
        EnergyBarInfluence newInfluence = ObjectPool.SharedInstance.SpawnPooledObject(ElementsEnum.ENERGY_INFLUENCE).GetComponent<EnergyBarInfluence>();
        newInfluence.transform.SetParent(energyBar.transform.Find("Energy Influences"), false);
        newInfluence.transform.localPosition = new Vector3((valuePosition * 10) + 1, 0, 0);
		newInfluence.SetIsIntensifying(isIntensifying);
		newInfluence.SetValuePosition(valuePosition);

		energyBarInfluences.Add(newInfluence);
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

        GameObject barMask = energyBar;
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
		Energy energy = collider.gameObject.GetComponent<Energy>();
		energy.Disappear(energyForceField, true);
		StageController.controller.EnergyCaught();

		// Add energy bar alteration
		InfluenceEnergyBar(energy.GetValue());
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
				// Same for ship debris particles
				spaceshipDebrisParticles.material.SetColor("_DissolveColorOuter", new Color(4.5f, 0, 0, 1));
				spaceshipDebrisParticles.material.SetColor("_DissolveColorMiddle", new Color(3.5f, 1f, 0, 1));
				spaceshipDebrisParticles.material.SetColor("_DissolveColorInner", new Color(2.5f, 2f, 0, 1));
				StageController.controller.DestroyShip();
			}
			else if (this.value > ValueRange.controller.GetMaxValue()) {
				// Make burning animation positive influenced
				burningAnimationSpriteRenderer.material.SetColor("_DissolveColorOuter", new Color(0, 2.5f, 4.5f, 1));
				burningAnimationSpriteRenderer.material.SetColor("_DissolveColorMiddle", new Color(0, 2f, 3.5f, 1));
				burningAnimationSpriteRenderer.material.SetColor("_DissolveColorInner", new Color(0, 1.5f, 2.5f, 1));
				// Same for ship debris particles
				spaceshipDebrisParticles.material.SetColor("_DissolveColorOuter", new Color(0, 2.5f, 4.5f, 1));
				spaceshipDebrisParticles.material.SetColor("_DissolveColorMiddle", new Color(0, 2f, 3.5f, 1));
				spaceshipDebrisParticles.material.SetColor("_DissolveColorInner", new Color(0, 1.5f, 2.5f, 1));
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

	public void SetLightIntensity(float intensity) {
		shipLightObject.intensity = intensity;
    }

	private void TurnOffLight() {
        //transform.Find("Point Light 2D").gameObject.SetActive(false);
        shipLightObject.gameObject.SetActive(false);

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
		burningAnimationSpriteRenderer.material.SetVector("_Seed", new Vector4(Random.Range(0, 1f), Random.Range(0, 1f), 0, 0));
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

		// Apply force to objects near the explosion
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, SHIP_EXPLOSION_RADIUS);

		foreach (Collider2D nearbyObject in colliders) {
			Rigidbody2D body2D = nearbyObject.GetComponent<Rigidbody2D>();

			if (body2D != null) {
				Vector3 distance = nearbyObject.transform.position - transform.position;
				body2D.AddForce(distance / distance.sqrMagnitude, ForceMode2D.Impulse);
				if (nearbyObject.tag == "Mine") {
					// Mines should explode if too close to the blast
					body2D.transform.parent.gameObject.GetComponent<EnergyMine>().Explode();
				}
			}
		}

		// Unleash debris particles
		spaceshipDebrisParticles.gameObject.SetActive(true);
	}

	/*
	 * Bullet time stuff
	 */
	float BULLET_TIME_SPEED_PARTICLE_SCALE = 10f;
	public void ActivateBulletTime() {
		TimeController.controller.SetTimeScale(0.2f);
		bulletTimeActive = true;
		MusicController.controller.SetMusicVolumeInGame(0.4f);

		// TODO Test for speed particles
		foreach (ParticleSystem ps in speedParticles) {
			ps.GetComponent<ParticleSystemRenderer>().velocityScale /= BULLET_TIME_SPEED_PARTICLE_SCALE;
			ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = ps.sizeOverLifetime;
			sizeOverLifetime.enabled = true;
        }
	}

	public void DeactivateBulletTime() {
		bulletTimeActive = false;
		MusicController.controller.SetMusicVolumeInGame(1);

		// Return time to normal
		TimeController.controller.SetTimeScale(1);

		// TODO Test for speed particles
		foreach (ParticleSystem ps in speedParticles) {
			ps.GetComponent<ParticleSystemRenderer>().velocityScale *= BULLET_TIME_SPEED_PARTICLE_SCALE;
			ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = ps.sizeOverLifetime;
			sizeOverLifetime.enabled = false;
		}
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

	public float GetTargetPosition() {
		return targetPosition;
    }
}
