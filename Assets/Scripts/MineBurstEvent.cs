using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineBurstEvent : ForegroundEvent {

    // Constants
    private const int MIN_ELEMENTS_NEARBY = 4;
    private const int MAX_ELEMENTS_NEARBY = 8;
    private const int TRIGGER_ELEMENT_CHANCE = 80;
    private const int TRIGGER_ELEMENT_TRIGGER_CHANCE = 80;
    private const float MIN_SQUARE_DISTANCE_BETWEEN_ELEMENTS = 1.2f;
    private const float MIN_MINE_DISTANCE = 1.45f;

    private float maxMineDistance = EnergyMine.EXPLOSION_RADIUS;

    void Start() {
        // Add mine at the center
        ObjectPool.SharedInstance.SpawnPooledObject(ElementsEnum.ENERGY_MINE, transform.position, GameObjectUtil.GenerateRandomRotation());

        // Define what is around
        DefineElementsAround();

        // Define if an element is coming to hit
        if (GameController.RollChance(TRIGGER_ELEMENT_CHANCE)) {
            GameObject triggerElement = CreateTriggerElement();

            MovingObject movingScript = triggerElement.AddComponent<MovingObject>();
            // Define if element is going to hit
            if (GameController.RollChance(TRIGGER_ELEMENT_TRIGGER_CHANCE)) {
                // Element is headed towards mine
                movingScript.Speed = (transform.position - triggerElement.transform.position);
            }
            else {
                // Element will miss mine
                Vector3 missingDistance = (GameController.RollChance(50) ? Vector3.up  : Vector3.down) * 2;
                movingScript.Speed = (transform.position + missingDistance - triggerElement.transform.position);
            }
        }

        // At the end, destroy itself
        Destroy(gameObject);
    }

    void DefineElementsAround() {
        int numElements = Random.Range(MIN_ELEMENTS_NEARBY, MAX_ELEMENTS_NEARBY);
        Vector3[] positions = new Vector3[numElements];
        for (int i = 0; i < positions.Length; i++) {
            positions[i] = transform.position;
        }


        for (int i = 0; i < numElements; i++) {
            Vector3 newPosition = new Vector3();
            int tries = 0;
            do {
                newPosition = GeneratePosition();
                tries++;
            } while (!CheckIfValidPosition(newPosition, positions) && tries < 3);

            if (tries < 3) {
                positions[i] = newPosition;
                GenerateElement(newPosition);
            }
        }
    }

    Vector3 GeneratePosition() {
        return transform.position + Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector3.right * Random.Range(MIN_MINE_DISTANCE, maxMineDistance);
    }

    bool CheckIfValidPosition(Vector3 position, Vector3[] positionsToTest) {
        bool valid = true;
        foreach (Vector3 positionToTest in positionsToTest) {
            if ((position - positionToTest).sqrMagnitude < MIN_SQUARE_DISTANCE_BETWEEN_ELEMENTS) {
                valid = false;
                break;
            }
        }

        return valid;
    }

    void GenerateElement(Vector3 position) {
        ElementsEnum[] possibleElements = new ElementsEnum[] {
            ElementsEnum.POSITIVE_ENERGY,
            ElementsEnum.NEGATIVE_ENERGY,
            ElementsEnum.ASTEROID,
            ElementsEnum.DEBRIS,
            ElementsEnum.ENERGY_MINE};

        ObjectPool.SharedInstance.SpawnPooledObject(possibleElements[Random.Range(0, possibleElements.Length - 1)], position, GameObjectUtil.GenerateRandomRotation());
    }

    GameObject CreateTriggerElement() {
        ElementsEnum[] possibleElements = new ElementsEnum[] {
            ElementsEnum.POSITIVE_ENERGY,
            ElementsEnum.NEGATIVE_ENERGY,
            ElementsEnum.ASTEROID,
            ElementsEnum.DEBRIS};

        return ObjectPool.SharedInstance.SpawnPooledObject(possibleElements[Random.Range(0, possibleElements.Length - 1)], 
            transform.position + new Vector3(Random.Range(1.5f, 3), Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax()), 0), 
            GameObjectUtil.GenerateRandomRotation());
    }
}
