using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Amount for rotating formation is the amount of energy pairs
 */
public class RotatingFormation : Formation {
    // Size and speed
    private const float MIN_ROTATION_SPEED = 60;
    private const float MAX_STARTING_ROTATION_SPEED = 360;
    private const float DEFAULT_MAX_ROTATION_SPEED = 1080;
    private const float MIN_RADIUS = 1f;
    private const float MAX_RADIUS = 1.5f;

    // Amounts
    private const int LOW_AMOUNT = 1;
    private const int MIN_MEDIUM_AMOUNT = 2;
    private const int MAX_MEDIUM_AMOUNT = 3;
    private const int HIGH_AMOUNT = 4;

    RotatingObject rotatingScript = null;

    float radius;

    float maxRotatingSpeed = DEFAULT_MAX_ROTATION_SPEED;

    void Start() {
        // TODO Decide if it should start at end speed or remain accelerating
        rotatingScript = GetComponent<RotatingObject>();
        rotatingScript.SetMinSpeed(MIN_ROTATION_SPEED);
        rotatingScript.SetMaxSpeed(MAX_STARTING_ROTATION_SPEED);

        // Mount energies
        GenerateEnergies();
    }

    // Update is called once per frame
    void Update()
    {
            float rotatingSpeed = rotatingScript.GetSpeed();
            if (rotatingSpeed > 0) {
                rotatingScript.SetSpeed(Mathf.Lerp(rotatingSpeed, maxRotatingSpeed, Time.deltaTime));
            }
            else {
                rotatingScript.SetSpeed(Mathf.Lerp(rotatingSpeed, -maxRotatingSpeed, Time.deltaTime));
            }
    }

    public override void ImpactFormation() {
        // Convert angular speed to linear speed
        float linearSpeed = rotatingScript.GetSpeed() / 360 * 2 * Mathf.PI * radius;

        for (int i = transform.childCount - 1; i >= 0; i--) {
            Transform child = transform.GetChild(i);
            // Using position to account for local rotation of the formation
            Vector3 perpendicularVector = child.position - transform.position;
            perpendicularVector = new Vector3(-perpendicularVector.y, perpendicularVector.x, 0);

            child.parent = transform.parent;

            // Make remaining elements continue in the direction
            child.GetComponent<Rigidbody2D>().AddForce(perpendicularVector * linearSpeed);
            child.GetComponent<IMovingObject>().enabled = true;
        }
        rotatingScript.enabled = false;
        enabled = false;
    }

    void GenerateEnergies() {
        // Define formation radius
        radius = Random.Range(MIN_RADIUS, MAX_RADIUS);

        // Since amount counts the pairs used, add energies using the real amount
        int energiesAmount = amount * 2;

        // Add energies
        bool currentEnergyIsPositive = GameController.RollChance(50);
        Vector3 angledRadius = Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector3.right * radius;
        for (int i = 0; i < energiesAmount; i++) {
            // Define type
            ElementsEnum type;
            if (currentEnergyIsPositive) {
                type = ElementsEnum.POSITIVE_ENERGY;
            } else {
                type = ElementsEnum.NEGATIVE_ENERGY;
            }
            GameObject newEnergy = ObjectPool.SharedInstance.SpawnPooledObject(type, transform.position + angledRadius, GameObjectUtil.GenerateRandomRotation());
            newEnergy.transform.parent = transform;

            // Check if there is a next energy to prepare
            if (i != energiesAmount - 1) {
                angledRadius = Quaternion.AngleAxis(360 / energiesAmount, Vector3.forward) * angledRadius;

                // Next energy has to be different
                currentEnergyIsPositive = !currentEnergyIsPositive;
            }
        }
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

    public void SetMaxRotatingSpeed(float maxRotatingSpeed) {
        this.maxRotatingSpeed = maxRotatingSpeed;
    }
}
