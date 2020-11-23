using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingFormation : Formation {

    private const float MIN_ROTATION_SPEED = 50;
    private const float MAX_STARTING_ROTATION_SPEED = 200;
    private const float MAX_ROTATION_SPEED = 1000;

    //float rotatingSpeed;

    RotatingObject rotatingScript = null;

    void Awake() {
        // Add rotating script if there is none
        if (GetComponent<RotatingObject>() == null) { 
            rotatingScript = gameObject.AddComponent<RotatingObject>(); 
        } else {
            rotatingScript = GetComponent<RotatingObject>();
        }
        rotatingScript.SetMinSpeed(MIN_ROTATION_SPEED);
        rotatingScript.SetMaxSpeed(MAX_STARTING_ROTATION_SPEED);
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
}
