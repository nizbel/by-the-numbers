using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RadialFormation : Formation
{
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
    private const int MIN_LOW_AMOUNT = 3;
    private const int MAX_LOW_AMOUNT = 7;
    private const int MIN_MEDIUM_AMOUNT = 8;
    private const int MAX_MEDIUM_AMOUNT = 16;
    private const int MIN_HIGH_AMOUNT = 17;
    private const int MAX_HIGH_AMOUNT = 30;

    private const int MIN_AMOUNT_FOR_DOUBLE_DECKER = 8;

    bool doubleDecker = false;

    // Start is called before the first frame update
    void Start() {
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
        if (amount >= MIN_AMOUNT_FOR_DOUBLE_DECKER) {
            doubleDecker = amount >= 12 ? true : GameController.RollChance(50);
        }

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
        centerEnergy = newEnergy.GetComponent<Energy>();

        // Invert type
        type = type == ElementsEnum.POSITIVE_ENERGY ? ElementsEnum.NEGATIVE_ENERGY : ElementsEnum.POSITIVE_ENERGY;
        // Inner circle
        int innerAmount = doubleDecker ? Mathf.FloorToInt(0.45f * (amount - 1)) : amount - 1;

        float amountFactor = (innerAmount + 1) / (2 * Mathf.PI);
        float radius = Random.Range(Mathf.Max(MIN_RADIUS_SIZE, amountFactor), 
            Mathf.Min(Mathf.Max(MIN_RADIUS_SIZE, amountFactor * 1.5f), MAX_RADIUS_SIZE));

        GenerateEnergyCircle(radius, type, innerAmount);

        // Outer circle, if available
        if (doubleDecker) {
            // Invert type again
            type = type == ElementsEnum.POSITIVE_ENERGY ? ElementsEnum.NEGATIVE_ENERGY : ElementsEnum.POSITIVE_ENERGY;

            int outerAmount = amount - innerAmount - 1;

            amountFactor = (outerAmount + 1) / (2 * Mathf.PI);
            radius = Random.Range(Mathf.Max(radius + MIN_RADIUS_SIZE, amountFactor), 
                Mathf.Min(radius + amountFactor * 1.5f, radius + MAX_RADIUS_SIZE));
            GenerateEnergyCircle(radius, type, outerAmount);
        }

    }

    void GenerateEnergyCircle(float radius, ElementsEnum energyType, int amount) {
        // Add energies
        Vector3 angledRadius = Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector3.right * radius;
        for (int i = 0; i < amount; i++) {
            GameObject newEnergy = ObjectPool.SharedInstance.SpawnPooledObject(energyType, transform.position + angledRadius, GameObjectUtil.GenerateRandomRotation());
            newEnergy.transform.parent = transform;

            // Check if there is a next energy to prepare
            if (i != amount - 1) {
                angledRadius = Quaternion.AngleAxis(360 / amount, Vector3.forward) * angledRadius;
            }
        }
    }

    public override float GetScreenOffset() {
        if (doubleDecker) {
            // Prepare for double decker
            return MAX_RADIUS_SIZE * 2;
        }
        return MAX_RADIUS_SIZE;
    }

    public override void SetAmount(ElementsAmount amount) {
        switch (amount) {
            case ElementsAmount.Low:
                this.amount = Random.Range(MIN_LOW_AMOUNT, MAX_LOW_AMOUNT);
                break;
            case ElementsAmount.Medium:
                this.amount = Random.Range(MIN_MEDIUM_AMOUNT, MAX_MEDIUM_AMOUNT);
                break;
            case ElementsAmount.High:
                this.amount = Random.Range(MIN_HIGH_AMOUNT, MAX_HIGH_AMOUNT);
                break;
        }
    }
}
