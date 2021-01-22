using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialInOutMovement : MonoBehaviour
{
    Vector3 outerPosition;

    Vector3 innerPosition;

    float movementSpeed;

    bool movingIn = true;

    // Update is called once per frame
    void FixedUpdate() {

        float inicio = Time.realtimeSinceStartup;
        if (movingIn) {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, innerPosition, Time.deltaTime * movementSpeed);
            if (transform.localPosition == innerPosition) {
                movingIn = false;
                Debug.Log((Time.realtimeSinceStartup - inicio));
            }
        }
        else {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, outerPosition, Time.deltaTime * movementSpeed);
            if (transform.localPosition == outerPosition) {
                movingIn = true;
            }
        }
    }

    /*
     * Getters and Setters
     */
    public Vector3 InnerPosition { get => innerPosition; set => innerPosition = value; }
    public Vector3 OuterPosition { get => outerPosition; set => outerPosition = value; }
    public float MovementSpeed { get => movementSpeed; set => movementSpeed = value; }
}
