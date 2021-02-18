using UnityEngine;
using System.Collections;

public class SpecialEventControllerDay9 : MonoBehaviour {

    private const float DEFAULT_ASTEROID_SHOWER_DURATION = 8f;
    private const float DEFAULT_DEBRIS_SHOWER_DURATION = 6f;

    /*
     * Speeches
     */
    [SerializeField]
    private Speech incomingSpeech;

    private int eventCode;

    public int EventCode { get => eventCode; set => eventCode = value; }

    float duration;

    /*
	 * Meteor generator prefab
	 */
    public GameObject meteorGeneratorPrefab;

    // Current generators available
    MeteorGenerator meteorGenerator = null;
    MeteorGenerator debrisGenerator = null;

    Coroutine generation;

    // Use this for initialization
    void Start() {
        // Fill duration
        duration = StageController.controller.GetCurrentMomentDuration();
        PlayNarrator();
        StartMeteorShower();
        StartDebrisShower();

        generation = StartCoroutine(GenerateShowers());
    }

    private void FixedUpdate() {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) {
            StopCoroutine(generation);
            Destroy(gameObject);
        }
    }

    void StartMeteorShower() {
        // Spawn meteor generator
        // Define a radial position from the middle horizontal right
        float angle = Random.Range(-0.25f, 0.25f) * Mathf.PI;
        float x = GameController.GetCameraXMax() * 1.25f + Mathf.Cos(angle) * GameController.GetCameraYMax();
        float y = Mathf.Sin(angle) * GameController.GetCameraYMax();
        Vector3 spawnPosition = new Vector3(x, y, 0);

        // Spawn element
        meteorGenerator = GameObject.Instantiate(meteorGeneratorPrefab, spawnPosition, Quaternion.identity).GetComponent<MeteorGenerator>();

        // Add duration to generator
        TimedDurationObject durationScript = meteorGenerator.gameObject.AddComponent<TimedDurationObject>();
        durationScript.Duration = Mathf.Min(duration, DEFAULT_ASTEROID_SHOWER_DURATION);
        durationScript.WaitTime = ShowerEvent.SHOWER_WARNING_PERIOD;
        durationScript.Activate();
        // Make meteor generator activate after wait time
        meteorGenerator.enabled = false;
        durationScript.AddOnWaitListener(meteorGenerator.Enable);

        // Play warning
        StageController.controller.PanelWarnDanger();
    }

    void StartDebrisShower() {
        // Spawn debris generator
        // Define a radial position from the middle horizontal right
        float angle = Random.Range(-0.25f, 0.25f) * Mathf.PI;
        float x = GameController.GetCameraXMax() * 1.25f + Mathf.Cos(angle) * GameController.GetCameraYMax();
        float y = Mathf.Sin(angle) * GameController.GetCameraYMax();
        Vector3 spawnPosition = new Vector3(x, y, 0);

        // Spawn element
        debrisGenerator = GameObject.Instantiate(meteorGeneratorPrefab, spawnPosition, Quaternion.identity).GetComponent<MeteorGenerator>();

        debrisGenerator.SetElementType(ElementsEnum.DEBRIS);

        // Add duration to generator
        TimedDurationObject durationScript = debrisGenerator.gameObject.AddComponent<TimedDurationObject>();
        // Debris take longer to start and last lesser
        durationScript.Duration = Mathf.Min(duration, DEFAULT_DEBRIS_SHOWER_DURATION);
        durationScript.WaitTime = ShowerEvent.SHOWER_WARNING_PERIOD*2;
        durationScript.Activate();
        // Make meteor generator activate after wait time
        debrisGenerator.enabled = false;
        durationScript.AddOnWaitListener(debrisGenerator.Enable);
    }

    IEnumerator GenerateShowers() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(DEFAULT_DEBRIS_SHOWER_DURATION / 2, DEFAULT_ASTEROID_SHOWER_DURATION / 2));
            if (meteorGenerator == null) {
                StartMeteorShower();
            }
            if (debrisGenerator == null) {
                StartDebrisShower();
            }
        }
    }

    void PlayNarrator() {
        NarratorController.controller.StartMomentSpeech(incomingSpeech);
    }
}