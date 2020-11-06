using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyWall : MonoBehaviour
{
    private const float WAIT_TIME = 0.3f;
    private const float DEFAULT_DISTANCE = 2.25f;

    /*
	 * Energy prefabs
	 */
    public GameObject positiveEnergyPrefab;
    public GameObject negativeEnergyPrefab;

    private int size = 0;

    List<Transform> energyTransforms = new List<Transform>();

    Vector3 speed = Vector3.up;

    float waitTime = WAIT_TIME;

    // Start is called before the first frame update
    void Start()
    {
        // Choose size
        if (size == 0) {
            DefineSize();
        }
        // TODO Set if moving

    }

    // Update is called once per frame
    void Update()
    {
        if (energyTransforms.Count == 0) {
            Destroy(gameObject);
        } else if (energyTransforms[0] == null) {
            energyTransforms.RemoveAt(0);
        }

        foreach (Transform childTransform in energyTransforms) {
            if (childTransform != null) {
                childTransform.position = Vector3.Lerp(childTransform.position, childTransform.position + speed, Time.deltaTime);
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
        size = Random.Range(3, 10);
        Vector3 currentPosition = transform.position;
        Vector3 currentOffset = Vector3.zero;

        // Size is dictated by center, then upper and bottom sequentially
        while (energyTransforms.Count < size) {
            Transform childTransform = AddEnergy();
            childTransform.position = currentPosition + currentOffset;
            currentPosition = childTransform.position;

            if (currentOffset.y > 0) {
                currentOffset += Vector3.up * DEFAULT_DISTANCE;
                currentOffset *= -1;
            } else if (currentOffset.y < 0) {
                currentOffset -= Vector3.up * DEFAULT_DISTANCE;
                currentOffset *= -1;
            } else {
                currentOffset += Vector3.up * DEFAULT_DISTANCE;
            }
        }
    }

    Transform AddEnergy() {
        GameObject prefab;
        if (GameController.RollChance(50)) {
            prefab = positiveEnergyPrefab;
        } else {
            prefab = negativeEnergyPrefab;
        }

        Transform newTransform = GameObject.Instantiate(prefab).transform;
        energyTransforms.Add(newTransform);

        return newTransform;
    }
}
