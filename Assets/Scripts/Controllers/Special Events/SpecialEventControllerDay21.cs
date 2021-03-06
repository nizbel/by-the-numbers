﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpecialEventControllerDay21 : MonoBehaviour {

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

        // Choose type at random
        if (GameController.RollChance(50)) {
            waterfall.SetFlowType(EnergyWaterfall.DOWNWARD);
        } else {
            waterfall.SetFlowType(EnergyWaterfall.UPWARD);
        }
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