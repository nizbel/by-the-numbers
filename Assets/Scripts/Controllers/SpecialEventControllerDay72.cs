using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Rendering.Universal;
using System.Collections.Generic;

public class SpecialEventControllerDay72 : MonoBehaviour {

    private const int TELLING_DREAM = 1;
    private const int AVOIDING_METEORS = 2;

    private int currentDay;

    private int eventCode;

    public int CurrentDay { get => currentDay; set => currentDay = value; }
    public int EventCode { get => eventCode; set => eventCode = value; }

    private float waitTime = 1;

    private int state;

    /*
	 * Meteor generator prefab
	 */
    public GameObject meteorGenerator;

    private List<GameObject> currentMeteorGenerators = new List<GameObject>();

    private float randomOffset = 0;

    List<string> speeches = null;

    // Use this for initialization
    void Start() {

        // Load list of speeches
        speeches = new List<string> {"Day 72 - Wreck of a dream", "Day 72 - Gloomy" , "Day 72 - Vision at the mirror" ,
            "Day 72 - The weird man" , "Day 72 - Continue" };

        ExplainDream();
    }

    // Update is called once per frame
    void Update() {
        if (speeches.Count > 0) {
            if (NarratorController.controller.GetState() != NarratorController.IMPORTANT && state == TELLING_DREAM) {
                StageController.controller.PanelWarnDanger();
                StartMeteorRain();

                // Last rain is doubled
                if (speeches.Count == 1) {
                    StartMeteorRain();
                }
            } else if (currentMeteorGenerators.Count == 0 && state == AVOIDING_METEORS) {
                ExplainDream();
            } else if (currentMeteorGenerators.Count > 0 && state == AVOIDING_METEORS) {
                for (int i = currentMeteorGenerators.Count-1; i >= 0; i--) {
                    if (currentMeteorGenerators[i] == null) {
                        currentMeteorGenerators.RemoveAt(i);
                    }
                }
            }
        } else {
            Destroy(gameObject);
        }

    }

    void ExplainDream() {
        NarratorController.controller.StartMomentSpeech(speeches[0]);
        speeches.RemoveAt(0);

        state = TELLING_DREAM;
    }

    void StartMeteorRain() {
        // Spawn meteor generator
        // Define a radial position from the middle horizontal right
        float angle = Random.Range(-0.25f, 0.25f) * Mathf.PI;
        float x = GameController.GetCameraXMax() + Mathf.Cos(angle) * GameController.GetCameraYMax();
        float y = Mathf.Sin(angle) * GameController.GetCameraYMax();
        Vector3 spawnPosition = new Vector3(x, y, 0);

        // Spawn element
        GameObject generator = (GameObject)Instantiate(meteorGenerator, spawnPosition, new Quaternion(0, 0, 0, 1));

        // Add duration to generator
        TimedDurationObject durationScript = generator.AddComponent<TimedDurationObject>();
        durationScript.Duration = 5;
        durationScript.WaitTime = 1.2f;
        // Make meteor generator activate after wait time
        generator.GetComponent<MeteorGenerator>().enabled = false;
        durationScript.AddOnWaitListener(generator.GetComponent<MeteorGenerator>().Enable);

        // Add generator to control list
        currentMeteorGenerators.Add(generator);

        // Set current state
        state = AVOIDING_METEORS;
    }

}