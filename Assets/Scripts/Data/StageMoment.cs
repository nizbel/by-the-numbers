using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageMoment {

    /*
     * Moment constants
     */
    // Unplayable
    public const int TYPE_CUTSCENE = 1;
    // Playable
    public const int TYPE_GAMEPLAY = 2;

    // Spawn Events
    public const int NO_SPAWN = 1;
    public const int COMMON_RANDOM_SPAWN = 2;
    public const int OBSTACLE_GALORE = 3;
    public const int ENERGY_GALORE = 4;

    // Duration in timestamp format
    [Tooltip("Duration in seconds")]
    public string duration;

    // Moment state
    public int momentState;

    // Type of moment
    public int type;

    [Tooltip("Speeches used during moment, one after the other")]
    public List<string> speeches;

    private float durationInSeconds = 0;

    private float startTime = 0;

    public bool hasMagneticBarriers = false;

    public int specialEvent = 0;

    // Chance of 4, 3 and 2 energies respectively
    public int[] energySpawnChances;

    public float obstacleSpawnChance = -1;

    // Chance of debris, meteors and stray engines
    public int[] obstacleChancesByType = null;

    public List<ElementSpawnChance> elementsSpawnChance = null;

    // Spawn interval type
    public int spawnInterval = ForegroundElementGenerator.DEFAULT_SPAWN_INTERVAL_TYPE;

    public float GetDurationInSeconds() {
        return durationInSeconds;
    }

    public float CalculateDurationInSeconds() {
        durationInSeconds = TimeUtil.ConvertTimestampToSeconds(duration);
        return durationInSeconds;
    }

    public float GetStartTime() {
        return startTime;
    }

    public void SetStartTime(float startTime) {
        this.startTime = startTime;
    }
}