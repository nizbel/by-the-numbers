using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyWaterfall : MonoBehaviour {

    private const float DEFAULT_WAIT_TIME = 0.6f;
    private const float DEFAULT_ENERGY_SPEED = 16f;
    private const float DEFAULT_Y_START_OFFSET = 2.5f;

    // Constants to control strength of the event
    public const int AMOUNT_WEAK = 3;
    public const int AMOUNT_STRONG = 4;

    // Constants for direction of flow
    public const int UPWARD = 1;
    public const int DOWNWARD = 2;
    public const int BOTH = 3;

    private float waitTime = DEFAULT_WAIT_TIME;

    int energiesPerRound = AMOUNT_STRONG;

    int flowType = DOWNWARD;

    private List<Vector3> startingPositions = new List<Vector3>();

    (float, float) positionLimitsX;

    // Start is called before the first frame update
    void Start()
    {
        positionLimitsX = (GameController.GetCameraXMin() + 6, GameController.GetCameraXMax() + 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (waitTime > 0) {
            waitTime -= Time.deltaTime;
            if (startingPositions.Count < energiesPerRound) {
                startingPositions.Add(GenerateStartingPosition());
            }
        }
        else {
            // Spawn energy waterfall
            SpawnWaterfall();
            startingPositions.Clear();

            // Reload wait time
            waitTime = DEFAULT_WAIT_TIME + Random.Range(-0.2f, 0.2f);
        }
    }

    Vector3 GenerateStartingPosition() {
        float positionX = Random.Range(positionLimitsX.Item1, positionLimitsX.Item2);

        switch (flowType) {
            case UPWARD:
                return new Vector3(positionX, GameController.GetCameraYMin() - DEFAULT_Y_START_OFFSET, 0);

            case DOWNWARD:
                return new Vector3(positionX, GameController.GetCameraYMax() + DEFAULT_Y_START_OFFSET, 0);

            case BOTH:
                if (GameController.RollChance(50)) {
                    return new Vector3(positionX, GameController.GetCameraYMax() + DEFAULT_Y_START_OFFSET, 0);
                }
                else {
                    return new Vector3(positionX, GameController.GetCameraYMin() - DEFAULT_Y_START_OFFSET, 0);
                }

            default:
                return new Vector3(positionX, GameController.GetCameraYMin() - DEFAULT_Y_START_OFFSET, 0);
        }
    }

    void SpawnWaterfall() {
        string binaryHolder = System.Convert.ToString(Random.Range(0, (int)Mathf.Pow(2, energiesPerRound)), 2).PadLeft(energiesPerRound, '0');

        int binaryIndex = 0;
        foreach (Vector3 position in startingPositions) {
            int chosenEnergy = 0;
            if (binaryHolder[binaryIndex] == '1') {
                // Generate positive
                chosenEnergy = ObjectPool.POSITIVE_ENERGY;
            }
            else {
                // Generate negative
                chosenEnergy = ObjectPool.NEGATIVE_ENERGY;
            }

            // Use binary index to change position
            Vector3 offset;
            Vector3 speed;
            if (position.y > 0) {
                offset = Vector3.up * binaryIndex;
                speed = new Vector3(PlayerController.controller.GetSpeed() - 3, -DEFAULT_ENERGY_SPEED, 0);
            } else {
                offset = Vector3.down * binaryIndex;
                speed = new Vector3(PlayerController.controller.GetSpeed() - 3, DEFAULT_ENERGY_SPEED, 0);
            }

            GameObject newEnergy = ObjectPool.SharedInstance.SpawnPooledObject(chosenEnergy, position + offset, GameObjectUtil.GenerateRandomRotation());

            MovingObject movingScript = newEnergy.AddComponent<MovingObject>();
            // Set direction
            movingScript.Speed = speed;

            // Move binary index
            binaryIndex++;
        }
    }

    public void SetEnergiesPerRound(int energiesPerRound) {
        this.energiesPerRound = energiesPerRound;
    }

    public void SetFlowType(int flowType) {
        this.flowType = flowType;
    }
}
