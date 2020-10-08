using UnityEngine;
using System.Collections;

public static class TimeUtil  {

    public static int ConvertTimestampToSeconds(string timestamp) {
        string[] timestampParts = timestamp.Split(':');
        return int.Parse(timestampParts[0]) * 60 + int.Parse(timestampParts[1]);
    }
}