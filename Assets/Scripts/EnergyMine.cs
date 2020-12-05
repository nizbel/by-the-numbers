using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyMine : MonoBehaviour
{
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

    // TODO Find a better way of making it proportional to force field radius
    float explosionRadius = 2.5f;

    // Between -1, 0 and 1
    int currentEnergy = 0;

    void Awake() {

    }

    // Start is called before the first frame update
    void Start()
    {
        // TODO Test if should energize at start
    }

    // Update is called once per frame
    void Update()
    {
        
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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D nearbyObject in colliders) {
            Rigidbody2D body2D = nearbyObject.GetComponent<Rigidbody2D>();

            if (body2D != null) {
                Vector3 distance = nearbyObject.transform.position - transform.position;
                body2D.AddForce(distance * (explosionRadius * explosionRadius - distance.sqrMagnitude));
                if (nearbyObject.tag == "Player") {
                    // Player dies if too close to the blast
                    StageController.controller.GetCurrentForegroundLayer().SetPlayerSpeed(0);
                    StageController.controller.DestroyShip();
                } else if (nearbyObject.tag == "Mine") {
                    // Mines should explode if too close to the blast
                    body2D.transform.parent.gameObject.GetComponent<EnergyMine>().Explode();
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
