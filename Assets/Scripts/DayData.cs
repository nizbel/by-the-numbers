using System;
using System.Collections.Generic;

[Serializable]
public class DayData {
    public List<StageMoment> startingMoments;
    public List<StageMoment> gameplayMoments;
    public List<StageMoment> endingMoments;
}