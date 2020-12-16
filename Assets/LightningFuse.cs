using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningFuse : MonoBehaviour
{
    private const float DEFAULT_CHARGING_TIME = 0.8f;

    private const int IDLE = 0;
    private const int ROTATING = 1;
    private const int CHARGING = 2;
    private const int FIRING = 3;

    GameObject lightningPrefab;

    float chargingTime = DEFAULT_CHARGING_TIME;

    SuddenRotatingElement rotationScript;

    int state = ROTATING;

    ParticleSystem chargeSignal;

    Transform energyStrikeTransform;

    void Awake() {
        // Define random rotation
        transform.Rotate(Vector3.forward * Random.Range(0, 360));
    }

    // Start is called before the first frame update
    void Start()
    {
        rotationScript = GetComponent<SuddenRotatingElement>();
        energyStrikeTransform = transform.Find("Energy Strike");
        chargeSignal = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(state) {
            case IDLE:
                StartRotationCountdown();
                break;

            case ROTATING:
                if (!rotationScript.enabled) {
                    // Check if can charge
                    if (CanCharge()) {
                        Charge();
                    } else {
                        state = IDLE;
                    }
                }
                break;

            case CHARGING:
                chargingTime -= Time.deltaTime;
                if (chargingTime <= 0) {
                    Fire();
                    chargingTime = DEFAULT_CHARGING_TIME;
                }
                break;

            case FIRING:
                if (energyStrikeTransform.localScale.x < 9.95f) {
                    energyStrikeTransform.localScale = Vector3.Lerp(energyStrikeTransform.localScale, new Vector3(10, 1, 1), Time.deltaTime * 10.5f);
                } else {
                    energyStrikeTransform.localScale = Vector3.Lerp(energyStrikeTransform.localScale, new Vector3(10, 0, 1), Time.deltaTime * 17.5f);
                    if (energyStrikeTransform.localScale.y < 0.05f) {
                        energyStrikeTransform.gameObject.SetActive(false);
                        state = IDLE;
                    }
                }
                break;
        }
    }

    public void Explode() {
        energyStrikeTransform.localScale = Vector3.zero;
        energyStrikeTransform.gameObject.SetActive(false);
        enabled = false;
    }

    bool CanCharge() {
        // Denies charging and shooting if position is too vertical, thus disabling the ship to evade
        if (transform.rotation.eulerAngles.z > 80 && transform.rotation.eulerAngles.z < 100) {
            return false;
        } else if (transform.rotation.eulerAngles.z > 260 && transform.rotation.eulerAngles.z < 280) {
            return false;
        }
        return true;
    }

    void Charge() {
        // Launch lightning signal
        // TODO Change charging signal color depending on energy
        chargeSignal.Play();

        state = CHARGING;
    }

    void Fire() {
        // TODO Generate lightning ray
        energyStrikeTransform.gameObject.SetActive(true);
        energyStrikeTransform.localScale = Vector3.zero;

        state = FIRING;
    }

    void StartRotationCountdown() {
        rotationScript.enabled = true;
        state = ROTATING;
    }
}
