using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingFormation : Formation {

    private const float MIN_ROTATION_SPEED = 50;
    private const float MAX_ROTATION_SPEED = 1000;
    private const float MAX_STARTING_ROTATION_SPEED = 200;

    //float rotatingSpeed;

    RotatingObject rotatingScript = null;

    void Awake() {
        // TODO Decide if it should start at end speed or remain accelerating
        rotatingScript = GetComponent<RotatingObject>();
        rotatingScript.SetMinSpeed(MIN_ROTATION_SPEED);
        rotatingScript.SetMaxSpeed(MAX_STARTING_ROTATION_SPEED);

        // Mount energies
        GenerateEnergies();

        SetCooldown(0.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set current angle
        transform.rotation = GameObjectUtil.GenerateRandomRotation();
    }

    // Update is called once per frame
    void Update()
    {
        float rotatingSpeed = rotatingScript.GetSpeed();
        if (rotatingSpeed > 0) {
            rotatingScript.SetSpeed(Mathf.Lerp(rotatingSpeed, MAX_ROTATION_SPEED, Time.deltaTime));
        } else {
            rotatingScript.SetSpeed(Mathf.Lerp(rotatingSpeed, -MAX_ROTATION_SPEED, Time.deltaTime));
        }

        // Check if none of its children have entered a reaction
        bool formationChanged = false;
        foreach (Transform child in transform) {
            if (child.GetComponent<EnergyReactionPart>() != null) {
                formationChanged = true;
                break;
            }
        }
        if (formationChanged) {
            foreach (Transform child in transform) {
                if (child.GetComponent<EnergyReactionPart>() == null) {
                    Vector3 perpendicularVector = child.position - transform.position;
                    perpendicularVector = new Vector3(-perpendicularVector.y * 2, 2 * perpendicularVector.x, 0);
                    child.GetComponent<Rigidbody2D>().AddForce(perpendicularVector * rotatingSpeed/200);
                }
            }
            rotatingScript.enabled = false;
            this.enabled = false;
        }
    }

    void GenerateEnergies() {
        // Choose amount of energies (2 to 6)
        int amount = Random.Range(1, 4) * 2;

        // Define formation radius
        float radius = Random.Range(1f, 1.5f);

        // Add energies
        bool currentEnergyIsPositive = GameController.RollChance(50);
        Vector3 angledRadius = Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector3.right * radius;
        for (int i = 0; i < amount; i++) {
            // Define type
            int type;
            if (currentEnergyIsPositive) {
                type = ObjectPool.POSITIVE_ENERGY;
            } else {
                type = ObjectPool.NEGATIVE_ENERGY;
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
