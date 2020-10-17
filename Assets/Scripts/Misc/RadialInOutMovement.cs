using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialInOutMovement : MonoBehaviour
{
    Vector3 outerPosition;

    Vector3 innerPosition;

    Vector3 movementSpeed = new Vector3();

    bool movingIn = true;

    // Update is called once per frame
    void FixedUpdate() {

        if (movingIn) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition - movementSpeed, Time.deltaTime);
            if (transform.localPosition.magnitude <= innerPosition.magnitude) {
                movingIn = false;
                transform.localPosition = innerPosition;
            }
        }
        else {
            transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + movementSpeed, Time.deltaTime);
            if (transform.localPosition.magnitude >= outerPosition.magnitude) {
                movingIn = true;
                transform.localPosition = outerPosition;
            }
        }

    }

    /*
     * Getters and Setters
     */
    public Vector3 InnerPosition { get => innerPosition; set => innerPosition = value; }
    public Vector3 OuterPosition { get => outerPosition; set => outerPosition = value; }
    public Vector3 MovementSpeed { get => movementSpeed; set => movementSpeed = value; }
}
