using System;

[Serializable]
public class SpeechPart {

    public string timestamp;

    public string text;

    private float timestampInSeconds = -1;

    public float GetTimestampInSeconds() {
        return timestampInSeconds;
    }

    public float CalculateTimestampInSeconds() {
        timestampInSeconds = TimeUtil.ConvertTimestampToSeconds(timestamp);
        return timestampInSeconds;
    }
}