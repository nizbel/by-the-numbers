using UnityEngine;
using UnityEditor;

[System.Serializable]
public class StageInfo {
    public int day = 0;

    public int highScore = 0;

    public int assistHighScore = 0;

    public bool played = false;

    public StageInfo(int day) {
        this.day = day;
    }

    public void UpdateHighScore(int score) {
        if (score > highScore) {
            highScore = score;
        }
    }
    public void UpdateAssistHighScore(int score) {
        if (score > assistHighScore) {
            assistHighScore = score;
        }
    }
}