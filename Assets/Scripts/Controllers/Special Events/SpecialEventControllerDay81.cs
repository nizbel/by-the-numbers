using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpecialEventControllerDay81 : MonoBehaviour {

    private int eventCode;

    public int EventCode { get => eventCode; set => eventCode = value; }

    private float duration;

    // Waterfall prefab
    public GameObject waterfallPrefab;

    private EnergyWaterfall waterfall = null;

    // Use this for initialization
    void Start() {
        // Get duration of moment
        duration = StageController.controller.GetCurrentMomentDuration();

        // Create and start waterfall
        waterfall = GameObject.Instantiate(waterfallPrefab).GetComponent<EnergyWaterfall>();

        waterfall.SetFlowType(EnergyWaterfall.BOTH);
        // Set amount
        waterfall.SetEnergiesPerRound(EnergyWaterfall.AMOUNT_STRONG);
    }

    // Update is called once per frame
    void Update() {
        duration -= Time.deltaTime;
        if (duration <= 0) {
            Destroy(waterfall.gameObject);
            Destroy(gameObject);
        }
    }
}