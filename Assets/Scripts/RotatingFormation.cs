using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Amount for rotating formation is the amount of energy pairs
 */
public class RotatingFormation : Formation {

    private const float MIN_ROTATION_SPEED = 50;
    private const float MAX_ROTATION_SPEED = 1000;
    private const float MAX_STARTING_ROTATION_SPEED = 200;

    private const int MIN_PAIR_ENERGIES_AMOUNT = 1;
    private const int MAX_PAIR_ENERGIES_AMOUNT = 3;

    private const float MIN_RADIUS = 1f;
    private const float MAX_RADIUS = 1.5f;

    RotatingObject rotatingScript = null;

    float radius;

    void Awake() {
        // TODO Decide if it should start at end speed or remain accelerating
        rotatingScript = GetComponent<RotatingObject>();
        rotatingScript.SetMinSpeed(MIN_ROTATION_SPEED);
        rotatingScript.SetMaxSpeed(MAX_STARTING_ROTATION_SPEED);

        // Mount energies
        GenerateEnergies();

        // Set current angle
        transform.rotation = GameObjectUtil.GenerateRandomRotation();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (rotatingScript.enabled) {
            float rotatingSpeed = rotatingScript.GetSpeed();
            if (rotatingSpeed > 0) {
                rotatingScript.SetSpeed(Mathf.Lerp(rotatingSpeed, MAX_ROTATION_SPEED, Time.fixedDeltaTime));
            }
            else {
                rotatingScript.SetSpeed(Mathf.Lerp(rotatingSpeed, -MAX_ROTATION_SPEED, Time.fixedDeltaTime));
            }
        //}
    }

    public override void ImpactFormation() {
        // Convert angular speed to linear speed
        float linearSpeed = rotatingScript.GetSpeed() / 360 * 2 * Mathf.PI * radius;

        for (int i = transform.childCount - 1; i >= 0; i--) {
            Transform child = transform.GetChild(i);
            // Using position to account for local rotation of the formation
            Vector3 perpendicularVector = child.position - transform.position;
            perpendicularVector = new Vector3(-perpendicularVector.y, perpendicularVector.x, 0);

            // Make remaining elements continue in the direction
            child.GetComponent<Rigidbody2D>().AddForce(perpendicularVector * linearSpeed);

            child.parent = transform.parent;
        }
        rotatingScript.enabled = false;
        enabled = false;
    }

    void GenerateEnergies() {
        // Choose amount of energies (2 to 6)
        int amount = Random.Range(MIN_PAIR_ENERGIES_AMOUNT, MAX_PAIR_ENERGIES_AMOUNT + 1) * 2;

        // Define formation radius
        radius = Random.Range(MIN_RADIUS, MAX_RADIUS);

        // Add energies
        bool currentEnergyIsPositive = GameController.RollChance(50);
        Vector3 angledRadius = Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector3.right * radius;
        for (int i = 0; i < amount; i++) {
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
            if (i != amount - 1) {
                angledRadius = Quaternion.AngleAxis(360 / amount, Vector3.forward) * angledRadius;

                // Next energy has to be different
                currentEnergyIsPositive = !currentEnergyIsPositive;
            }
        }
    }
}
