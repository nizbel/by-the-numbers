using System;

[Serializable]
public class StageInfo {
    public int day = 0;

    public int highScore = 0;

    public int assistHighScore = 0;

    // Amount of tries
    public int tries = 0;

    // Amount of successful tries
    public int wins = 0;

    // Amount of successful tries in assist mode
    public int assistWins = 0;

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

    public bool IsDone() {
        return (wins > 0 || assistWins > 0);
    }

    public bool Played() {
        return tries > 0;
    }

    public float WinRate() {
        if (Played()) {
            return 100f * wins / tries;
        } else {
            return 0;
        }
    }
}