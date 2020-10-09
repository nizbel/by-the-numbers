using System;
using System.Collections.Generic;

[Serializable]
public class StageEvent {

    /*
     * Event constants
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

    // Event code
    public int eventState;

    // Type of event
    public int type;

    public bool spawnEnergyCloud;

    public List<string> speeches;

    private int durationInSeconds = 0;

    private float startTime = 0;

    public int GetDurationInSeconds() {
        return durationInSeconds;
    }

    public int CalculateDurationInSeconds() {
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