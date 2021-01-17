using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/DayData")]
public class DayData : ScriptableObject {
    public int day = 0;

    public List<StageMoment> startingMoments;
    public List<StageMoment> gameplayMoments;
    public List<StageMoment> endingMoments;

    public int startingShipValue = 0;

    // Shows the starting center position of the value range
    public int startingValueRange = 0;

    // Chance to observe a constellation
    public float constellationChance = 33.33f;

    public List<ElementsEnum> elementsInDay = new List<ElementsEnum>();

    public List<ElementsEnum> GetElementsInDay() {
        List<ElementsEnum> elementsSpawnedInDay = new List<ElementsEnum>();

        foreach (StageMoment moment in gameplayMoments) {
            foreach (ElementSpawnChance spawnChance in moment.elementsSpawnChance) {
                if (!elementsSpawnedInDay.Contains(spawnChance.element)) {
                    elementsSpawnedInDay.Add(spawnChance.element);
                }
            }

        }

        return elementsSpawnedInDay;
    }
}