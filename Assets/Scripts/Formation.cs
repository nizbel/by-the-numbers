﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation : MonoBehaviour {
    public enum ElementsAmount {
        Low,
        Medium,
        High
    }

    // Elements that will compose formation
    protected ElementsEnum[] elementTypes;

    // The energy at the center keeps the formation in place
    protected Transform centerElement = null;

    protected int amount = 0;

    // Elements should start not being removable by world position
    // Used if formation is going to start from behind the player
    protected bool nonDestructibleAtStart = false;

    // Should add screen offset to position after creation?
    protected bool shouldApplyScreenOffset = true;

    public virtual float GetScreenOffset() {
        return 0;
    }

    public virtual void ImpactFormation() {
        if (centerElement == null) {
            float linearSpeed = 0;

            RotatingObject rotatingScript = gameObject.GetComponent<RotatingObject>();
            if (rotatingScript != null) {
                // Convert angular speed to linear speed
                linearSpeed = rotatingScript.GetSpeed() / 360 * 2 * Mathf.PI;
            }

            // Change energies' parent and move them away
            for (int i = transform.childCount-1; i >= 0; i--) {
                Transform child = transform.GetChild(i);
                RadialInOutMovement radialInOutScript = child.GetComponent<RadialInOutMovement>();
                if (radialInOutScript != null) {
                    Destroy(radialInOutScript);
                }

                Rigidbody2D childRigidbody = child.GetComponent<Rigidbody2D>();
                if (childRigidbody != null) {
                    if (linearSpeed != 0) {
                        // Using position to account for local rotation of the formation
                        Vector3 perpendicularVector = child.position - transform.position;
                        perpendicularVector = new Vector3(-perpendicularVector.y, perpendicularVector.x, 0);

                        childRigidbody.AddForce(perpendicularVector * linearSpeed);
                    }
                    childRigidbody.AddForce(child.position - transform.position);
                }

                // If elements started indestructible, set them destructible now
                if (nonDestructibleAtStart) {
                    child.GetComponent<DestructibleObject>().SetIsDestructibleNow(true);
                }

                child.parent = transform.parent;

                // Enable movement after changing parent
                // TODO Find a better way to ensure OnEnable is called
                child.GetComponent<IMovingObject>().enabled = false;
                child.GetComponent<IMovingObject>().enabled = true;
            }
        }
    }

    public Transform GetCenterElement() {
        return centerElement;
    }

    public void SetCenterElement(Transform centerElement) {
        this.centerElement = centerElement;
    }

    public virtual void SetAmount(ElementsAmount amount) {

    }

    public void SetAmount(int amount) {
        this.amount = amount;
    }

    public void SetElementTypes(ElementsEnum[] elementTypes) {
        this.elementTypes = elementTypes;
    }

    public void SetNonDestructibleAtStart(bool nonDestructibleAtStart) {
        this.nonDestructibleAtStart = nonDestructibleAtStart;
    }

    public void SetShouldApplyScreenOffset(bool shouldApplyScreenOffset) {
        this.shouldApplyScreenOffset = shouldApplyScreenOffset;
    }

    // TODO Change name from destructible to removable
    public void SetElementsDestructibleNow() {
        foreach (Transform child in transform) {
            child.GetComponent<DestructibleObject>().SetIsDestructibleNow(true);
        }
    }
}
