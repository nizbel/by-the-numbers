using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageMoment {
    public string description;

    // Duration in timestamp format
    [Tooltip("Duration in seconds")]
    public float duration;

    // Moment state
    public MomentSpawnStateEnum momentState;

    // Type of moment
    public MomentTypeEnum type;

    [Tooltip("Speeches used during moment, one after the other")]
    public List<string> speeches;

    private float startTime = 0;

    // TODO Remove this attribute
    public bool hasMagneticBarriers = false;

    public int specialEvent = 0;

    // Chance of 4, 3 and 2 energies respectively
    public int[] energySpawnChances;

    public float obstacleSpawnChance = -1;

    // Chance of debris, meteors and stray engines
    public int[] obstacleChancesByType = new int[0];

    [Tooltip("Chance of spawning foreground elements that aren't energies")]
    public List<ElementSpawnChance> elementsSpawnChance = new List<ElementSpawnChance>();

    // Spawn interval type
    public int spawnInterval = ForegroundElementGenerator.DEFAULT_SPAWN_INTERVAL_TYPE;

    public float GetStartTime() {
        return startTime;
    }

    public void SetStartTime(float startTime) {
        this.startTime = startTime;
    }
}