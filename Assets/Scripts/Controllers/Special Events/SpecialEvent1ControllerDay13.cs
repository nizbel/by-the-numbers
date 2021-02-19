using UnityEngine;
using System.Collections;

public class SpecialEvent1ControllerDay13 : MonoBehaviour {

    private int eventCode;

    public int EventCode { get => eventCode; set => eventCode = value; }

    /*
	 * Meteor generator prefab
	 */
    public GameObject meteorGenerator;

    // Use this for initialization
    void Start() {
        StartMeteorShower();
        Destroy(gameObject);
    }


    void StartMeteorShower() {
        // Spawn meteor generator
        // Define a radial position from the middle horizontal right
        float angle = Random.Range(-0.25f, 0.25f) * Mathf.PI;
        float x = GameController.GetCameraXMax() * 1.25f + Mathf.Cos(angle) * GameController.GetCameraYMax();
        float y = Mathf.Sin(angle) * GameController.GetCameraYMax();
        Vector3 spawnPosition = new Vector3(x, y, 0);

        // Spawn element
        MeteorGenerator generator = GameObject.Instantiate(meteorGenerator, spawnPosition, Quaternion.identity).GetComponent<MeteorGenerator>();

        // Add duration to generator
        TimedDurationObject durationScript = generator.gameObject.AddComponent<TimedDurationObject>();
        durationScript.Duration = StageController.controller.GetCurrentMomentDuration();
        durationScript.WaitTime = ShowerEvent.SHOWER_WARNING_PERIOD;
        durationScript.Activate();
        // Make meteor generator activate after wait time
        generator.enabled = false;
        durationScript.AddOnWaitListener(generator.Enable);

        // Play warning
        StageController.controller.PanelWarnDanger();
    }
}