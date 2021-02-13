using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour
{
    public void ProcessCollisionBase() {
        // Checks if energy belongs in a formation, if true remove it
        Formation parentFormation = transform.parent.GetComponent<Formation>();
        if (parentFormation != null) {
            transform.parent = parentFormation.transform.parent;

            if (this == parentFormation.GetCenterEnergy()) {
                parentFormation.SetCenterEnergy(null);
            }
            parentFormation.ImpactFormation();
        }

        RemoveMovementScripts();
    }

    // Remove energy movement scripts
    private void RemoveMovementScripts() {
        MovingObject movingScript = GetComponent<MovingObject>();
        if (movingScript != null) {
            Destroy(movingScript);
        }
        RotatingObject rotatingScript = GetComponent<RotatingObject>();
        if (rotatingScript != null) {
            Destroy(rotatingScript);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        ProcessCollisionBase();
    }
}
