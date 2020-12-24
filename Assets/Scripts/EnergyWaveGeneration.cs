using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyWaveGeneration : MonoBehaviour
{
    private const float DEFAULT_GENERATION_INTERVAL = 0.2f;

    [SerializeField]
    GameObject positiveEnergyPrefab;
    [SerializeField]
    GameObject negativeEnergyPrefab;

    float nextGeneration = DEFAULT_GENERATION_INTERVAL;

    float currentPositionY;
    int direction = 1;
    bool canChange = true;

    // Start is called before the first frame update
    void Start()
    {
        DefinePositionY();
    }

    // Update is called once per frame
    void Update()
    {
        nextGeneration -= Time.deltaTime;
        if (nextGeneration <= 0) {
            nextGeneration = DEFAULT_GENERATION_INTERVAL;

            Vector3 nextPosition = new Vector3(GameController.GetCameraXMax() + 1, 0, 0);

            // Add two energies
            //GameObject positiveEnergy = GameObject.Instantiate(positiveEnergyPrefab);
            GameObject positiveEnergy = ObjectPool.SharedInstance.SpawnPooledObject(ObjectPool.POSITIVE_ENERGY);
            positiveEnergy.transform.position = nextPosition + Vector3.up * currentPositionY * direction;
            //GameObject negativeEnergy = GameObject.Instantiate(negativeEnergyPrefab);
            GameObject negativeEnergy = ObjectPool.SharedInstance.SpawnPooledObject(ObjectPool.NEGATIVE_ENERGY);
            negativeEnergy.transform.position = nextPosition + Vector3.up * -currentPositionY * direction;

            // Define next Y position
            DefinePositionY();
        }
    }

    void DefinePositionY() {
        currentPositionY = (Mathf.Sin(Time.time*6.5f) + 1) / 2 * (GameController.GetCameraYMax() - 1) + 0.5f;
        if (currentPositionY <= 1f && canChange) {
            direction *= -1;
            canChange = false;
        } else if (currentPositionY >= GameController.GetCameraYMax() - 1f && !canChange) {
            canChange = true;
        }
    }
}
