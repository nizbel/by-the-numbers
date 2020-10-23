using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameInfo {
    public List<StageInfo> listStageInfo = new List<StageInfo>();

    public int infiniteHighScore = 0;

    public StageInfo GetStageInfoByDay(int day) {
        foreach (StageInfo stage in listStageInfo) {
            if (stage.day == day) {
                return stage;
            }
        }
        StageInfo newStageInfo = new StageInfo(day);
        listStageInfo.Add(newStageInfo);
        return newStageInfo;
    }

    public bool StageDone(int day) {
        StageInfo stageInfo = GetStageInfoByDay(day);
        return stageInfo.highScore > 0 || stageInfo.assistHighScore > 0;
    }

    public bool StagePlayed(int day) {
        StageInfo stageInfo = GetStageInfoByDay(day);
        return stageInfo.played;
    }
}