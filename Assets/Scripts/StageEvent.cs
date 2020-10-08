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

    // Events
    public const int COMMON_RANDOM_SPAWN_STATE = 1;
    public const int OBSTACLE_GALORE_STATE = 2;
    public const int OPERATION_BLOCK_GALORE_STATE = 3;

    // Duration in timestamp format
    public string duration;

    // TODO for now apply eventCode into StageController's state
    // Event code
    public int eventCode;

    // Type of event
    public int type;

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