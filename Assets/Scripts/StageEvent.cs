using System;
using System.Collections.Generic;

[Serializable]
public class StageEvent {

    /*
     * Event constants
     */
    // Unplayable
    public const int TYPE_CUTSCENE = 1;
    // Playable, nothing that kills the player spawns
    public const int TYPE_CALM_MOMENT = 2;
    // Playable, spawns anything
    public const int TYPE_ACTION_MOMENT = 3;

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