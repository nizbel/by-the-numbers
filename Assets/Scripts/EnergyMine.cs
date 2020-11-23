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

    float explosionRadius = 4.5f;

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

        // Play explosion sound
        audioSource.clip = explosionSound;
        audioSource.loop = false;
        audioSource.Play();

        // TODO Create explosion animation

        // TODO Apply forces on nearby objects
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D nearbyObject in colliders) {
            Rigidbody2D body2D = nearbyObject.GetComponent<Rigidbody2D>();

            if (body2D != null) {
                Vector3 distance = nearbyObject.transform.position - transform.position;
                body2D.AddForce(distance * (explosionRadius * explosionRadius - distance.sqrMagnitude)/100, ForceMode2D.Impulse);
                if (nearbyObject.tag == "Player") {
                    StageController.controller.DestroyShip();
                }
            }
        }

        // TODO Allow object to be removed from OutScreenDestroyerController list

        // TODO Disappear
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }
    }

    public void EnergizeOnCollision(OperationBlock energy) {
        Energize(energy.GetValue() > 0);

        // TODO Animate energy reaction like with player ship

        // TODO Disappear energy
    }

    void Energize(bool positiveEnergy) {
        currentEnergy = (positiveEnergy ? 1 : -1);
        if (positiveEnergy) {
            // Energize with positive
            forceField.GetComponent<ParticleSystemRenderer>().material.SetColor("Color", forceFieldPositiveColor);
        } else {
            // Energize with negative
            forceField.GetComponent<ParticleSystemRenderer>().material.SetColor("Color", forceFieldNegativeColor);
        }
        forceField.gameObject.SetActive(true);
    }

    /*
     * Getters and Setters
     */
    public int GetCurrentEnergy() {
        return currentEnergy;
    }
}
