using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyWall : MonoBehaviour
{
    private const float WAIT_TIME = 0.3f;
    private const float DEFAULT_DISTANCE = 2.25f;
    public const int MAX_SIZE = 9;

    public const int RANDOM_ENERGIES = 1;
    public const int POSITIVE_ENERGIES = 2;
    public const int NEGATIVE_ENERGIES = 3;

    private int size = 0;

    List<Transform> energyTransforms = new List<Transform>();

    bool moving = false;

    Vector3 speed = Vector3.up;

    int type = RANDOM_ENERGIES;

    // Distance between energies
    float distance = DEFAULT_DISTANCE;

    float waitTime = WAIT_TIME;

    // Start is called before the first frame update
    void Start()
    {
        // Choose size
        if (size == 0) {
            DefineSize();
        }

        GenerateWall();
    }

    // Update is called once per frame
    void Update()
    {
        if (energyTransforms.Count == 0) {
            Destroy(gameObject);
        } else if (energyTransforms[0] == null) {
            energyTransforms.RemoveAt(0);
        }

        if (moving) {
            foreach (Transform childTransform in energyTransforms) {
                if (childTransform != null) {
                    childTransform.position = Vector3.Lerp(childTransform.position, childTransform.position + speed, Time.deltaTime);
                }
            }
        }

        if (waitTime <= 0) {
            speed *= -1;
            waitTime = WAIT_TIME;
        } else {
            waitTime -= Time.deltaTime;
        }
    }

    void DefineSize() {
        size = Random.Range(3, MAX_SIZE);
    }

    void GenerateWall() { 
        Vector3 currentPosition = transform.position;
        Vector3 currentOffset = Vector3.zero;

        // Size is dictated by center, then upper and bottom sequentially
        while (energyTransforms.Count < size) {
            Transform childTransform = AddEnergy();
            childTransform.position = currentPosition + currentOffset;
            currentPosition = childTransform.position;

            if (currentOffset.y > 0) {
                currentOffset += Vector3.up * distance;
                currentOffset *= -1;
            } else if (currentOffset.y < 0) {
                currentOffset -= Vector3.up * distance;
                currentOffset *= -1;
            } else {
                currentOffset += Vector3.up * distance;
            }
        }
    }

    Transform AddEnergy() {
        int chosenEnergy;
        if (type == RANDOM_ENERGIES) {
            if (GameController.RollChance(50)) {
                //prefab = positiveEnergyPrefab;
                chosenEnergy = ObjectPool.POSITIVE_ENERGY;
            }
            else {
                //prefab = negativeEnergyPrefab;
                chosenEnergy = ObjectPool.NEGATIVE_ENERGY;
            }
        } else if (type == POSITIVE_ENERGIES) {
            //prefab = positiveEnergyPrefab;
            chosenEnergy = ObjectPool.POSITIVE_ENERGY;
        } else {
            //prefab = negativeEnergyPrefab;
            chosenEnergy = ObjectPool.NEGATIVE_ENERGY;
        }

        //Transform newTransform = GameObject.Instantiate(prefab).transform;
        Transform newTransform = ObjectPool.SharedInstance.SpawnPooledObject(chosenEnergy).transform;
        energyTransforms.Add(newTransform);

        return newTransform;
    }

    /*
     * Getters and Setters
     */
    public bool GetMoving() {
        return moving;
    }

    public void SetMoving(bool moving) {
        this.moving = moving;
    }

    public void SetSize(int size) {
        this.size = size;
    }

    public void SetType(int type) {
        this.type = type;
    }

    public void SetDistance(float distance) {
        this.distance = distance;
    }
}
