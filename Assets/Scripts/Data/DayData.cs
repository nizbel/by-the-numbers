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

    // Chance to observe a constellation
    public float constellationChance = 33.33f;

    public List<ElementsEnum> GetElementsInDay() {
        List<ElementsEnum> elementsSpawnedInDay = new List<ElementsEnum>();

        foreach (StageMoment moment in gameplayMoments) {
            List<ElementsEnum> elementsSpawnedInMoment = new List<ElementsEnum>(moment.elementsSpawnChance.Keys);
            
            foreach (ElementsEnum element in elementsSpawnedInMoment) { 
                if (!elementsSpawnedInDay.Contains(element)) {
                    elementsSpawnedInDay.Add(element);
                } 
            }
        }

        return elementsSpawnedInDay;
    }
}