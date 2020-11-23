using System.Collections.Generic;
using System;

[Serializable]
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
        return GetStageInfoByDay(day).IsDone();
    }

    public bool StagePlayed(int day) {
        return GetStageInfoByDay(day).Played();
    }

    public List<StageInfo> ListStageInfoByDay() {
        List<StageInfo> orderedList = new List<StageInfo>();
        foreach (StageInfo stageInfo in listStageInfo) {
            bool added = false;
            for (int i = 0; i < orderedList.Count; i++) {
                if (orderedList[i].day > stageInfo.day) {
                    orderedList.Insert(i, stageInfo);
                    added = true;
                    break;
                }
            }
            if (!added) {
                orderedList.Add(stageInfo);
            }
        }
        return orderedList;
    }
}