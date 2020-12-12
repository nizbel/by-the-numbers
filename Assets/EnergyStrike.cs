using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyStrike : MonoBehaviour
{
    // TODO Allow value changing
    int value = -1;

    void OnTriggerEnter2D(Collider2D collider) {
        switch (collider.tag) {
            case "Energy":
                // TODO Check if energy value to move away or react
                break;

            case "Player":
                PlayerController.controller.UpdateShipValue(value * (StageController.SHIP_VALUE_LIMIT * 2 + 1));
                break;

            case "Indestructible Obstacle":
                break;

            default:
                if (collider.isTrigger) {
                    break;
                }
                DissolvingObject dissolveScript = collider.gameObject.AddComponent<DissolvingObject>();
                dissolveScript.SetDissolutionByEnergy(value);
                break;

        }
    }
}
