using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rotates object fast as to make a tilting animation towards a target angle
public class SuddenRotatingElement : MonoBehaviour
{
    private const float MIN_ROTATION_WAIT = 0.4f;
    private const float MAX_ROTATION_WAIT = 0.6f;

    private const float DEFAULT_ROTATION_OFFSET = 30;
    private const float DEFAULT_ROTATION_SPEED = 25;

    // Wait time before starting rotation
    float waitTime;

    bool isRotating = false;

    // Target angle
    float targetAngle;

    // Speed
    float rotationSpeed;

    // Offset for the target angle, so as to provide the tilting animation
    float rotationOffset;

    // Current target with offset
    float targetAngleWithOffset;

    // Keeps track of the current angle
    float currentAngle;

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnEnable() {
        DefineRotationSpeed();
        rotationOffset = DEFAULT_ROTATION_OFFSET;

        DefineWaitTime();
        DefineTargetAngle();
        DefineTargetAngleWithOffset();
    }

    // Update is called once per frame
    void Update()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0 && !isRotating) {
            isRotating = true;

            currentAngle = transform.rotation.eulerAngles.z;
        }
    }

    void FixedUpdate() {
        if (isRotating) {
            // Rotate and keep current angle updated
            transform.Rotate(0, 0, rotationSpeed);
            currentAngle += rotationSpeed;

            //Debug.Log(rotationSpeed + "..." + transform.rotation.eulerAngles.z + "..." + currentAngle + "..." + targetAngleWithOffset + "..." + targetAngle);
            if (PassedCurrentOffset()) {
                rotationOffset *= 0.5f;

                // If rotation offset is too low, stop
                if (rotationOffset < 0.1f) {
                    isRotating = false;
                    enabled = false;
                } else {
                    rotationSpeed *= -0.8f;
                    DefineTargetAngleWithOffset();
                }
            }

        }
    }

    bool PassedCurrentOffset() {
        if (rotationSpeed > 0) {
            return currentAngle > targetAngleWithOffset;
        } else {
            return currentAngle < targetAngleWithOffset;
        }
    }

    public void RestartRotation() {
        DefineWaitTime();
    }

    void DefineRotationSpeed() {
        if (GameController.RollChance(50)) {
            rotationSpeed = DEFAULT_ROTATION_SPEED;
        }
        else {
            rotationSpeed = -DEFAULT_ROTATION_SPEED;
        }
    }

    void DefineWaitTime() {
        waitTime = Random.Range(MIN_ROTATION_WAIT, MAX_ROTATION_WAIT);
    }

    void DefineTargetAngle() {
        if (rotationSpeed > 0) {
            targetAngle = (transform.rotation.eulerAngles.z + Random.Range(1, 3) * 45);
        } else {
            targetAngle = (transform.rotation.eulerAngles.z - Random.Range(1, 3) * 45);
        }
    }

    void DefineTargetAngleWithOffset() {
        // Set target angle in this frame
        if (rotationSpeed > 0) {
            targetAngleWithOffset = (targetAngle + rotationOffset);
        }
        else {
            targetAngleWithOffset = (targetAngle - rotationOffset);
        }
    }
}
