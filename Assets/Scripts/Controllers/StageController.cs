using UnityEngine;
using System.IO;
using UnityEngine.Experimental.Rendering.Universal;
using System.Collections.Generic;

public abstract class StageController : MonoBehaviour {

	// Constants
	public const int SHIP_VALUE_LIMIT = 15;
	protected const float GAME_OVER_DURATION = 1.2f;

	// Stage state constants
	public const int STARTING_STATE = 1;
	public const int GAMEPLAY_STATE = 2;
	public const int ENDING_STATE = 3;
	public const int GAME_OVER_STATE = 4;

	// Stage moments constants
	protected const string PATH_JSON_MOMENTS = "Json/Days/";

	protected int score = 0;

	protected int obstaclesPast = 0;

	protected int energiesCaught = 0;

	protected int magneticBarriersPast = 0;

	protected bool gamePaused = false;

	protected float gameOverTimer = GAME_OVER_DURATION;

	// Player data
	protected Transform playerShipTransform;

	// Score text object during stage
	protected TextMesh scoreText;
	protected bool showScore;

	// Keeps track of whether magnetic barriers are spawning
	protected bool magneticBarriersSpawning = false;

	// Current stage state
	// TODO Remove serializeField
	[SerializeField]
	protected int state = STARTING_STATE;

	// Current day special charges (used for special spawns)
	protected int currentSpecialCharges = 0;

	// Current moment
	protected StageMoment currentMoment = null;

	// Controls the foreground layers that moves objects in the foreground
	protected List<ForegroundLayer> foregroundLayers = new List<ForegroundLayer>();

	[SerializeField]
	protected GameObject objectPool = null;

	public static StageController controller;

	// TODO Find a better place for this
	public GameObject dissolvingParticlesPrefab;

	void Awake() {
        if (controller == null) {
            controller = this;

			// Prepare foreground layer
			AddForegroundLayer(FindObjectOfType<ForegroundLayer>());

			// Start object pooling
			objectPool.SetActive(true);
		}
        else {
            Destroy(gameObject);
        }
    }

    // Method for game over
    public void DestroyShip() {
        // TODO Remove test when going production
        //if (2 == 2) {
        //    return;
        //}

        // Tells narrator controller to stop
        NarratorController.controller.GameOver();

		// TODO play explosion sound

		PlayerController.controller.CrashAndBurn();

        state = GAME_OVER_STATE;

		ScreenFadeController.controller.StartFadeOut(ScreenFadeController.GAME_OVER_FADE_OUT_SPEED);
	}

	public void GameOver() {
		// Calls game controller for state change
		if (GameController.controller.GetState() == GameController.GAMEPLAY_STORY) {
			// Save game info
			GameController.controller.UpdateDayInfoDefeat();

			// End run
			GameController.controller.ChangeState(GameController.GAME_OVER_STORY);
		}
		else {
			// Save game info
			GameController.GetGameInfo().infiniteHighScore = score;
			GameController.controller.Save();

			// End run
			GameController.controller.ChangeState(GameController.GAME_OVER_INFINITE);
		}
	}

	// Method when player hits an energy
	public void EnergyCaught() {
		energiesCaught++;
		AddScore(1);
	}

	// Add amount to score
	public void AddScore(int amount) {
		score += amount;

		// Update score text
		if (showScore) {
			scoreText.text = score.ToString();
		}
    }


	// Player passed though magnetic barrier
	public void PastThroughMagneticBarrier() {
		magneticBarriersPast++;
		AddScore(1);
		GameController.GetCamera().GetComponent<CameraShake>().DefaultShake();
	}

	// Warn about incoming special event danger
	public void PanelWarnDanger() {
		// TODO Stop using Find()
		AudioSource panelWarning = playerShipTransform.Find("Panel").GetComponent<AudioSource>();
		panelWarning.Play();
    }

	// Pause game
	public void PauseGame() {
		Time.timeScale = 0;
		AudioListener.pause = true;
		gamePaused = true;
		InputController inputController = FindObjectOfType<InputController>();
		inputController.enabled = false;
	}

	// Resume game
	public void ResumeGame() {
		Time.timeScale = 1;
		AudioListener.pause = false;
		gamePaused = false;
		InputController inputController = FindObjectOfType<InputController>();
		inputController.enabled = true;
	}

	public void UseSpecialCharges(int chargesAmount) {
		currentSpecialCharges -= chargesAmount;
		Debug.Log("Charges left: " + currentSpecialCharges);
	}

	public void AddForegroundLayer(ForegroundLayer layer) {
		foregroundLayers.Add(layer);
		// Remove old layer
		if (foregroundLayers.Count > 1) {
			foregroundLayers.RemoveAt(0);
		}
    }

	public ForegroundLayer GetCurrentForegroundLayer() {
		return foregroundLayers[0];
    }

	public virtual void SkipCutscenes() {

    }

	// TODO Remove for production version
	public void SkipCurrentMoment() {
		currentMoment.SetStartTime(Time.time - currentMoment.duration);
    }

	public virtual float GetPlayableMomentsDuration() {
		return 0;
	}

	public virtual float TimeLeftBeforeNoSpawn() {
		return 0;
	}

	/*
	 * Getters and setters
	 */

	public int GetScore() {
		return score;
	}

	public int GetEnergiesCaught() {
		return energiesCaught;
	}

	public void SetEnergiesCaught(int energiesCaught) {
		this.energiesCaught = energiesCaught;
	}

	public int GetMagneticBarriersPast() {
		return magneticBarriersPast;
	}

	public void SetMagneticBarriersPast(int magneticBarriersPast) {
		this.magneticBarriersPast = magneticBarriersPast;
	}

	public Transform GetPlayerShipTransform() {
		return playerShipTransform;
	}


    public int GetState() {
        return state;
    }

	public int GetCurrentSpecialCharges() {
		return currentSpecialCharges;
    }

	public float GetCurrentMomentDuration() {
		if (currentMoment != null) {
			return currentMoment.duration;
		}
		return 0;
	}

	public float GetCurrentMomentStartTime() {
		if (currentMoment != null) {
			return currentMoment.GetStartTime();
		}
		return 0;
	}

	public MomentSpawnStateEnum GetCurrentMomentState() {
		if (currentMoment != null) {
			return currentMoment.momentState;
		}
		return MomentSpawnStateEnum.NoSpawn;
	}

	public MomentTypeEnum GetCurrentMomentType() {
		if (currentMoment != null) {
			return currentMoment.type;
		}
		return MomentTypeEnum.Cutscene;
	}

	public bool GetCurrentMomentDistantForegroundSpawn() {
		if (currentMoment != null) {
			return currentMoment.distantForegroundSpawn;
		}
		return true;
	}

	public bool GetGamePaused() {
		return gamePaused;
	}
}
