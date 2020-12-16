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
                // Calculate influence in energy by using its distance from the start of the strike
                Vector3 energyStrikeBeginning = transform.parent.position;
                // Find last bit of strike considering the position is as its center
                Vector3 energyStrikeVector = Quaternion.AngleAxis(transform.parent.localEulerAngles.z, Vector3.forward) * transform.localPosition * 2;
                // Perpendicular vector sending energy away
                Vector3 perpendicularVector = new Vector3(-energyStrikeVector.y, energyStrikeVector.x, 0);

                // Get energy direction
                Vector3 energyDirection = collider.transform.position - energyStrikeBeginning;

                // Angle that determines how much influence it will get from longitudinal vector and perpendicular vector
                float angle = Vector3.SignedAngle(energyStrikeVector, energyDirection, Vector3.forward) * Mathf.Deg2Rad;

                // Combine perpendicular influence with energy direction
                Vector3 resultingInfluence = energyDirection + perpendicularVector * Mathf.Sin(angle);
                
                if (energy.GetValue() * value > 0) {
                    // Blow away energy
                    collider.attachedRigidbody.AddForceAtPosition(resultingInfluence * 3.5f, collider.transform.position);

                    // Create energy shock effect
                    GameObject.Instantiate(energy.energyShock, collider.transform.position + Mathf.Sin(angle) * energyDirection.magnitude * perpendicularVector.normalized, 
                        Quaternion.AngleAxis(transform.parent.localEulerAngles.z, Vector3.forward));
                } else {
                    // TODO Explode energy
                    EnergyBurst energyBurst = collider.gameObject.AddComponent<EnergyBurst>();
                    energyBurst.SetEnergyBurstPrefab(energy.energyBurst);
                    energyBurst.SetPositiveEnergy(energy.GetValue() > 0);
                    energyBurst.SetBurstAngle(Vector3.SignedAngle(Vector3.right, resultingInfluence, Vector3.forward));

                    energy.DisappearInReaction();

                    collider.enabled = false;
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
