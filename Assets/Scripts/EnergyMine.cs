using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Subclass from DestructibleObject in order to make spawn/despawn preparations
public class EnergyMine : MonoBehaviour {
    // TODO Find a better way of making it proportional to force field radius
    public const float EXPLOSION_RADIUS = 2.5f;
    public const float SQUARED_CORE_EXPLOSION_RADIUS = 4f;

    [SerializeField]
    [ColorUsage(true,true)]
    Color forceFieldNegativeColor;

    [SerializeField]
    [ColorUsage(true, true)]
    Color forceFieldPositiveColor;

    [SerializeField]
    ParticleSystem forceField;

    // TODO Set explosion sound
    [SerializeField]
    AudioClip explosionSound;

    [SerializeField]
    AudioSource audioSource;

    // Between -1, 0 and 1
    int currentEnergy = 0;

    // Start is called before the first frame update
    void Start()
    {
        // TODO Test if should energize at start
    }

    public void Explode() {
        // Disable force field
        forceField.Stop();

        // Play explosion animation
        GetComponent<ParticleSystem>().Play();

        // Play explosion sound
        audioSource.clip = explosionSound;
        audioSource.loop = false;
        audioSource.Play();

        // TODO Disappear and disable colliders, explosives and force fields
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }

        // TODO Apply forces on nearby objects
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, EXPLOSION_RADIUS);

        foreach (Collider2D nearbyObject in colliders) {
            Rigidbody2D body2D = nearbyObject.GetComponent<Rigidbody2D>();

            if (body2D != null) {
                Vector3 distance = nearbyObject.transform.position - transform.position;
                body2D.AddForce(distance * (EXPLOSION_RADIUS * EXPLOSION_RADIUS - distance.sqrMagnitude));
                if (nearbyObject.tag == "Player") {
                    Debug.Log(distance.sqrMagnitude);
                    // Test if close enough to explode or just shake
                    if (distance.sqrMagnitude <= SQUARED_CORE_EXPLOSION_RADIUS) {
                        // Player dies if too close to the blast
                        StageController.controller.GetCurrentForegroundLayer().SetPlayerSpeed(0);
                        StageController.controller.DestroyShip();
                    } else {
                        // Camera shakes
                        GameController.GetCamera().GetComponent<CameraShake>().DefaultShake();
                        // TODO Add damage to the ship, move this to a player controlled class
                        Material spaceshipMaterial = PlayerController.controller.GetSpaceshipSpriteRenderer().material;
                        spaceshipMaterial.SetFloat("_CurrentDamage", Mathf.Min(spaceshipMaterial.GetFloat("_CurrentDamage") + 0.25f, 1));
                        GameController.SetShipDamage(spaceshipMaterial.GetFloat("_CurrentDamage"));
                    }
                } else if (nearbyObject.tag == "Mine") {
                    // Mines should explode if too close to the blast
                    body2D.transform.parent.gameObject.GetComponent<EnergyMine>().Explode();
                } else if (nearbyObject.tag == "Frail Obstacle") {
                    DissolvingObject dissolveScript = body2D.gameObject.AddComponent<DissolvingObject>();
                    dissolveScript.SetDissolutionByDamage();
                }
            }
        }

        // TODO Allow object to be removed from OutScreenDestroyerController list
    }

    public void EnergizeOnCollision(Collider2D energy) {
        Energize(energy.GetComponent<Energy>().GetValue());

        // TODO Animate energy reaction like with player ship

        // TODO Disappear energy
    }

    void Energize(int energyValue) {
        currentEnergy = energyValue;

        if (energyValue > 0) {
            // Energize with positive
            forceField.GetComponent<ParticleSystemRenderer>().material.SetColor("_Color", forceFieldPositiveColor);
        } else {
            // Energize with negative
            forceField.GetComponent<ParticleSystemRenderer>().material.SetColor("_Color", forceFieldNegativeColor);
        }

        // Activate trigger
        forceField.gameObject.SetActive(true);

        // Start force field effect
        forceField.GetComponent<ParticleSystem>().Play();
    }

    /*
     * Getters and Setters
     */
    public int GetCurrentEnergy() {
        return currentEnergy;
    }
}
