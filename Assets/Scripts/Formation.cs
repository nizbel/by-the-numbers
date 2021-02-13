using System.Collections;
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
    protected Energy centerEnergy = null;

    protected int amount;

    public virtual float GetScreenOffset() {
        return 0;
    }

    public virtual void ImpactFormation() {
        if (centerEnergy == null) {
            // Change energies' parent and move them away
            for (int i = transform.childCount-1; i >= 0; i--) {
                Transform child = transform.GetChild(i);
                RadialInOutMovement radialInOutScript = child.GetComponent<RadialInOutMovement>();
                if (radialInOutScript != null) {
                    Destroy(radialInOutScript);
                }

                Rigidbody2D childRigidbody = child.GetComponent<Rigidbody2D>();
                if (childRigidbody != null) {
                    childRigidbody.AddRelativeForce(child.localPosition);
                }

                child.parent = transform.parent;
            }
        }
    }

    public Energy GetCenterEnergy() {
        return centerEnergy;
    }

    public void SetCenterEnergy(Energy centerEnergy) {
        this.centerEnergy = centerEnergy;
    }

    public virtual void SetAmount(ElementsAmount amount) {

    }

    public void SetElementTypes(ElementsEnum[] elementTypes) {
        this.elementTypes = elementTypes;
    }
}
