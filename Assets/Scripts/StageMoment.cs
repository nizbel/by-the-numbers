using System;
using System.Collections.Generic;

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
    public const int OPERATION_BLOCK_GALORE = 4;

    // Duration in timestamp format
    public string duration;

    // Moment state
    public int momentState;

    // Type of moment
    public int type;

    public List<string> speeches;

    private float durationInSeconds = 0;

    private float startTime = 0;

    public bool hasRangeChangers = false;

    public int specialEvent = 0;

    // Chance of 4, 3 and 2 energies respectively
    public int[] energySpawnChances = null;

    public float obstacleSpawnChance = -1;

    // Chance of debris, meteors and stray engines
    public int[] obstacleChancesByType = null;

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