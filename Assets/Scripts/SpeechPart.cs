using System;

[Serializable]
public class SpeechPart {

    public string timestamp;

    public string text;

    private int timestampInSeconds = -1;

    public int GetTimestampInSeconds() {
        return timestampInSeconds;
    }

    public int CalculateTimestampInSeconds() {
        timestampInSeconds = TimeUtil.ConvertTimestampToSeconds(timestamp);
        return timestampInSeconds;
    }
}