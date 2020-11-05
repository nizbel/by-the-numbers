using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpecialEventControllerDay21 : MonoBehaviour {

    private const float DEFAULT_WAIT_TIME = 0.6f;

    private int eventCode;

    public int EventCode { get => eventCode; set => eventCode = value; }

    private float waitTime = DEFAULT_WAIT_TIME;

    /*
	 * Energy prefabs
	 */
    public GameObject positiveEnergyPrefab;
    public GameObject negativeEnergyPrefab;

    int energiesPerRound = 4;

    private List<Vector3> startingPositions = new List<Vector3>();

    private float duration;

    // Use this for initialization
    void Start() {
        // Get duration of moment
        duration = StageController.controller.GetCurrentMomentDuration();
    }

    // Update is called once per frame
    void Update() {
        if (waitTime > 0) {
            waitTime -= Time.deltaTime;
            if (startingPositions.Count < energiesPerRound) {
                startingPositions.Add(GenerateStartingPosition());
            }
        } else {
            // Spawn energy waterfall
            SpawnWaterfall();
            startingPositions.Clear();

            // Reload wait time
            waitTime = DEFAULT_WAIT_TIME + Random.Range(-0.2f, 0.2f);
        }

        duration -= Time.deltaTime;
        if (duration <= 0) {
            Destroy(gameObject);
        }
    }

    Vector3 GenerateStartingPosition() {
        float positionX = Random.Range(GameController.GetCameraXMin() + 4, GameController.GetCameraXMax()) + 2;
        if (GameController.RollChance(50)) {
            return new Vector3(positionX, GameController.GetCameraXMax() + 1, 0); 
        } else {
            return new Vector3(positionX, GameController.GetCameraXMin() - 1, 0);
        }
    }

    void SpawnWaterfall() {
        string binaryHolder = System.Convert.ToString(Random.Range(0, (int)Mathf.Pow(2, energiesPerRound)-1), 2).PadLeft(energiesPerRound, '0');

        float movementSpeed = 16;

        int binaryIndex = 0;
        foreach (Vector3 position in startingPositions) {
            GameObject chosenPrefab = null;
            if (binaryHolder[binaryIndex] == '1') {
                // Generate positive
                chosenPrefab = positiveEnergyPrefab;
            } else {
                // Generate negative
                chosenPrefab = negativeEnergyPrefab;
            }

            GameObject newEnergy = GameObject.Instantiate(chosenPrefab, position + Vector3.up * binaryIndex, GameObjectUtil.GenerateRandomRotation());

            // TODO Remove marking
            newEnergy.name += " Generated";

            MovingObject movingScript = newEnergy.AddComponent<MovingObject>();
            // Set direction
            if (position.y > 0) {
                movingScript.Speed = new Vector3(PlayerController.controller.GetSpeed() - 3, -movementSpeed, 0);
            } else {
                movingScript.Speed = new Vector3(PlayerController.controller.GetSpeed() - 3, movementSpeed, 0);
            }

            // Move binary index
            binaryIndex++;
        }

    }
}