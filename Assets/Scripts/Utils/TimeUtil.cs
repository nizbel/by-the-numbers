using UnityEngine;
using System.Collections;
using System;

public static class TimeUtil  {

    public static float ConvertTimestampToSeconds(string timestamp) {
        // If it contains '.', then it controls fractions of seconds
        string[] timestampParts = timestamp.Split(':');
        if (timestamp.Contains(".")) {
            return int.Parse(timestampParts[0]) * 60 + float.Parse(timestampParts[1]);
        } else {
            return int.Parse(timestampParts[0]) * 60 + int.Parse(timestampParts[1]);
        }
    }
    public static string ConvertSecondsToTimestamp(float duration) {
        int minutes = (int) (duration / 60);
        float seconds = duration % 60;

        return String.Format("{0}:{1:00.#}", minutes, seconds);
    }
}