using System;
using System.Collections.Generic;

[Serializable]
public class DayData {
    public List<StageMoment> startingMoments;
    public List<StageMoment> gameplayMoments;
    public List<StageMoment> endingMoments;

    public int startingShipValue = 0;

    // Shows the starting center position of the value range
    public int startingValueRange = 0;
}