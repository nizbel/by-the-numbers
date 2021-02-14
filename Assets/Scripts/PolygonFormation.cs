using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
 * Amount for polygon formation is the amount of sides
 */
public class PolygonFormation : Formation {
    // Size and speed
    private const float MAX_RADIUS_SIZE = 3.1f;
    private const float MIN_RADIUS_SIZE = 1.1f;
    private const float MAX_IN_OUT_RADIUS_SIZE = 2.5f;
    private const float MIN_IN_OUT_RADIUS_SIZE = 1f;
    private const float MAX_IN_OUT_SPEED = 0.4f;
    private const float MIN_IN_OUT_SPEED = 0.8f;
    private const float MIN_ROTATING_SPEED = 45f;
    private const float MAX_ROTATING_SPEED = 360f;

    // Chances
    private const float IN_OUT_MOVEMENT_CHANCE = 30f;
    private const float ROTATING_MOVEMENT_CHANCE = 30f;

    // Amounts
    private const int LOW_AMOUNT = 3;
    private const int MIN_MEDIUM_AMOUNT = 4;
    private const int MAX_MEDIUM_AMOUNT = 5;
    private const int HIGH_AMOUNT = 6;

    bool doubleDecker = false;

    // The initial angle for one of the sides
    float initialAngle;

    // Start is called before the first frame update
    void Start() {
        // Decide initial angle
        initialAngle = Random.Range(0, 360);

        // Mount energies
        GenerateEnergies();

        // Add screen offset
        transform.position += Vector3.right * GetScreenOffset();

        // Check if should rotate
        if (GameController.RollChance(ROTATING_MOVEMENT_CHANCE)) {
            RotatingObject rotatingScript = gameObject.AddComponent<RotatingObject>();
            rotatingScript.SetMinSpeed(MIN_ROTATING_SPEED);
            rotatingScript.SetMaxSpeed(MAX_ROTATING_SPEED);
        }

        // Check if energies will be moving in/out or not
        if (GameController.RollChance(IN_OUT_MOVEMENT_CHANCE)) {
            float radiusFactor = Random.Range(MIN_IN_OUT_RADIUS_SIZE, MAX_IN_OUT_RADIUS_SIZE);
            // Apply radius to children
            float movementSpeedFactor = Random.Range(MIN_IN_OUT_SPEED, MAX_IN_OUT_SPEED);
            if (doubleDecker) {
                movementSpeedFactor /= 2;
            }

            foreach (Transform child in transform) {
                // Not for child at center
                if (child.localPosition.x != 0 && child.localPosition.y != 0) {
                    RadialInOutMovement movScript = child.gameObject.AddComponent<RadialInOutMovement>();

                    // Define attributes of the in/out movement
                    movScript.InnerPosition = child.localPosition;

                    // Define outer position (max position for the movement)
                    child.localPosition *= radiusFactor;

                    movScript.MovementSpeed = movementSpeedFactor * child.localPosition.magnitude;
                    movScript.OuterPosition = child.localPosition;
                }
            }
        }
    }

    void GenerateEnergies() {
        doubleDecker = GameController.RollChance(50);        

        // Center energy
        bool centerEnergyIsPositive = GameController.RollChance(50);
        ElementsEnum type;
        if (centerEnergyIsPositive) {
            type = ElementsEnum.POSITIVE_ENERGY;
        }
        else {
            type = ElementsEnum.NEGATIVE_ENERGY;
        }
        GameObject newEnergy = ObjectPool.SharedInstance.SpawnPooledObject(type, transform.position, GameObjectUtil.GenerateRandomRotation());
        newEnergy.transform.parent = transform;
        centerElement = newEnergy.transform;

        // Invert type
        type = type == ElementsEnum.POSITIVE_ENERGY ? ElementsEnum.NEGATIVE_ENERGY : ElementsEnum.POSITIVE_ENERGY;

        // Inner polygon
        float amountFactor = (amount + 1) / (2 * Mathf.PI);
        float radius = Random.Range(Mathf.Max(MIN_RADIUS_SIZE, amountFactor),
            Mathf.Min(Mathf.Max(MIN_RADIUS_SIZE, amountFactor * 1.5f), MAX_RADIUS_SIZE));

        GenerateEnergyPoligon(radius, type, amount, amount);

        // Outer polygon, if available
        if (doubleDecker) {
            // Invert type again
            type = type == ElementsEnum.POSITIVE_ENERGY ? ElementsEnum.NEGATIVE_ENERGY : ElementsEnum.POSITIVE_ENERGY;

            amountFactor = (amount * 2 + 1) / (2 * Mathf.PI);
            radius = Random.Range(Mathf.Max(radius + MIN_RADIUS_SIZE, amountFactor),
                Mathf.Min(radius + amountFactor * 1.5f, radius + MAX_RADIUS_SIZE));
            GenerateEnergyPoligon(radius, type, amount * 2, amount);
        }

    }

    void GenerateEnergyPoligon(float radius, ElementsEnum energyType, int amount, int sides) {
        // Add energies
        Vector3 angledRadius = Quaternion.Euler(0, 0, initialAngle) * Vector3.right * radius;

        // If amount == sides, then it's the inner polygon
        if (amount == sides) {
            for (int i = 0; i < amount; i++) {
                GameObject newEnergy = ObjectPool.SharedInstance.SpawnPooledObject(energyType, transform.position + angledRadius, GameObjectUtil.GenerateRandomRotation());
                newEnergy.transform.parent = transform;

                // Check if there is a next energy to prepare
                if (i != amount - 1) {
                    angledRadius = Quaternion.AngleAxis(360 / amount, Vector3.forward) * angledRadius;
                }
            }
        }
        else {
            // Else, it's the outer polygon, with radius alternating between energies
            List<Vector3> positionsList = new List<Vector3>();

            int outerAmount = amount / 2;

            for (int i = 0; i < outerAmount; i++) {
                // Generate first the energies that are just an extension from the inner polygon
                GameObject newEnergy = ObjectPool.SharedInstance.SpawnPooledObject(energyType, transform.position + angledRadius, GameObjectUtil.GenerateRandomRotation());
                newEnergy.transform.parent = transform;

                positionsList.Add(newEnergy.transform.position);

                // Check if there is a next energy to prepare
                if (i != outerAmount - 1) {
                    angledRadius = Quaternion.AngleAxis(360 / outerAmount, Vector3.forward) * angledRadius;
                }
            }

            // Generate then the energies that stay amid every 2 energies in the outer polygon
            for (int i = 0; i < positionsList.Count; i++) {
                GameObject newEnergy;
                // Generate first the energies that are just an extension from the inner polygon
                if (i == positionsList.Count - 1) {
                    // The last one gets the difference between last and first energy
                    Vector3 differenceBetweenPositions = positionsList[0] - positionsList[i];
                    newEnergy = ObjectPool.SharedInstance.SpawnPooledObject(energyType, positionsList[i] + differenceBetweenPositions/2, GameObjectUtil.GenerateRandomRotation());
                }
                else {
                    Vector3 differenceBetweenPositions = positionsList[i+1] - positionsList[i];
                    newEnergy = ObjectPool.SharedInstance.SpawnPooledObject(energyType, positionsList[i] + differenceBetweenPositions / 2, GameObjectUtil.GenerateRandomRotation());
                }
                newEnergy.transform.parent = transform;
            }
        }
    }

    public override float GetScreenOffset() {
        // Prepare for double decker
        if (doubleDecker) {
            return MAX_RADIUS_SIZE * 2;
        }
        return MAX_RADIUS_SIZE;
    }


    public override void SetAmount(ElementsAmount amount) {
        switch (amount) {
            case ElementsAmount.Low:
                this.amount = LOW_AMOUNT;
                break;
            case ElementsAmount.Medium:
                this.amount = Random.Range(MIN_MEDIUM_AMOUNT, MAX_MEDIUM_AMOUNT);
                break;
            case ElementsAmount.High:
                this.amount = HIGH_AMOUNT;
                break;
        }
    }
}
