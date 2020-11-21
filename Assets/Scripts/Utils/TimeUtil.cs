using UnityEngine;
using System.Collections;

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
}