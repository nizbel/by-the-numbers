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
                Energy energy = collider.GetComponent<Energy>();
                if (energy.GetValue() * value > 0) {
                    // Blow away energy
                    Vector3 distanceEnergyCollision = collider.transform.position - transform.position;
                    collider.attachedRigidbody.AddForceAtPosition(distanceEnergyCollision, collider.transform.position);

                    // Create energy shock effect
                    Vector3 halfDistanceEnergyCollision = distanceEnergyCollision / 2;
                    // Get angle that is perpendicular to distance
                    float angleEnergyCollision = Vector3.SignedAngle(Vector3.right, halfDistanceEnergyCollision, Vector3.forward) + 90;
                    GameObject.Instantiate(energy.energyShock, transform.position + halfDistanceEnergyCollision, Quaternion.AngleAxis(angleEnergyCollision, Vector3.forward));
                } else {
                    // TODO Explode energy

                }
                break;

            case "Indestructible Obstacle":
                break;

            case "Lightning Fuse":
                LightningFuse lightningFuseScript = collider.gameObject.GetComponent<LightningFuse>();
                lightningFuseScript.Explode();
                DissolvingObject dissolveFuseScript = collider.gameObject.AddComponent<DissolvingObject>();
                dissolveFuseScript.SetDissolutionByEnergy(value);
                break;

            case "Mine":
                break;

            case "Player":
                PlayerController.controller.UpdateShipValue(value * (StageController.SHIP_VALUE_LIMIT * 2 + 1));
                break;

            default:
                if (collider.isTrigger) {
                    break;
                }
                DissolvingObject dissolveScript = collider.gameObject.AddComponent<DissolvingObject>();
                dissolveScript.SetDissolutionByEnergy(value);
                // TODO Disable shadow caster when dissolved
                break;

        }
    }

    public void SetValue(int value) {
        this.value = value;
    }
}
