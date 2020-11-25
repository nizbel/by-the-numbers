using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuddenRotatingElement : MonoBehaviour
{
    private const float MIN_ROTATION_WAIT = 0.4f;
    private const float MAX_ROTATION_WAIT = 0.6f;

    private const float DEFAULT_ROTATION_OFFSET = 30;
    private const float DEFAULT_ROTATION_SPEED = 25;

    float waitTime;

    bool isRotating = false;

    float rotationAngle;

    float rotationSpeed;

    float rotationOffset;

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnEnable() {
        DefineWaitTime();
        DefineRotationAngle();

        rotationSpeed = DEFAULT_ROTATION_SPEED;
        rotationOffset = DEFAULT_ROTATION_OFFSET;
    }

    // Update is called once per frame
    void Update()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0 && !isRotating) {
            isRotating = true;
        }
    }

    void FixedUpdate() {
        if (isRotating) {
            float angleToMove;
            if (rotationSpeed > 0) {
                angleToMove = rotationAngle + rotationOffset;
            } else {
                angleToMove = (rotationAngle - rotationOffset + 360) % 360;
            }
            //transform.Rotate(0, 0, Mathf.Lerp(transform.rotation.eulerAngles.z, rotationAngle + rotationOffset, rotationSpeed * Time.deltaTime));
            transform.Rotate(0, 0, rotationSpeed);

            Debug.Log(rotationSpeed + "..." + transform.rotation.eulerAngles.z + "..." + angleToMove + "..." + rotationAngle);
            if ((rotationSpeed > 0 && transform.rotation.eulerAngles.z > angleToMove) || (rotationSpeed < 0 && transform.rotation.eulerAngles.z < angleToMove)) {
                rotationOffset *= 0.5f;
                rotationSpeed *= -0.8f;
            }

            if (rotationOffset <= 0.1f) {
                isRotating = false;
                enabled = false;
            }
        }
    }

    public void RestartRotation() {
        DefineWaitTime();
    }

    void DefineWaitTime() {
        waitTime = Random.Range(MIN_ROTATION_WAIT, MAX_ROTATION_WAIT);
    }

    void DefineRotationAngle() {
        rotationAngle = (transform.rotation.eulerAngles.z + Random.Range(1, 3) * 45) % 360;
    }
}
