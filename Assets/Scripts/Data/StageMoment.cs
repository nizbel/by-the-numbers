using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageMoment {
    [Header("Basic info")]
    public string description;

    // Duration in timestamp format
    [Tooltip("Duration in seconds")]
    public float duration = 3;

    // Moment state
    public MomentSpawnStateEnum momentState = MomentSpawnStateEnum.CommonRandomSpawn;

    // Type of moment
    public MomentTypeEnum type = MomentTypeEnum.Gameplay;

    [Header("Speeches")]
    [Tooltip("Speeches used during moment, one after the other")]
    public List<string> speeches;
    public List<Speech> momentSpeeches;

    private float startTime = 0;

    [Header("Special event")]
    public int specialEvent = 0;
    public GameObject specialEventObject = null; 

    [Header("Spawn chances")]
    // Chance of 4, 3 and 2 energies respectively
    public int[] energySpawnChances;

    // Chance of debris, meteors and stray engines
    public int[] obstacleChancesByType = new int[0];

    [Tooltip("Chance of spawning foreground elements")]
    public List<ElementSpawnChance> elementsSpawnChance = new List<ElementSpawnChance>();

    // Keeps info on the available elements if necessary
    private ElementsEnum[] availableElements = null;

    [Tooltip("Spawn moving element chance")]
    [Range(0, 100)]
    public float spawnMovingElementChance = ForegroundElementGenerator.DEFAULT_MOVING_ELEMENT_CHANCE;

    [Header("Spawn intervals")]
    // Spawn interval type
    public SpawnIntervalEnum spawnInterval = SpawnIntervalEnum.Default;
    public DifficultyEnum magneticBarriersSpawnInterval = DifficultyEnum.Easy;

    [Tooltip("Should spawn in distant foreground?")]
    public bool distantForegroundSpawn = true;

    public float GetStartTime() {
        return startTime;
    }

    public void SetStartTime(float startTime) {
        this.startTime = startTime;
    }

    public bool HasMagneticBarriers() {
        foreach (ElementSpawnChance elementSpawn in elementsSpawnChance) {
            if (elementSpawn.element == ElementsEnum.MAGNETIC_BARRIER) {
                return true;
            }
        }
        return false;
    }

    public ElementsEnum[] GetAvailableElements() {
        if (availableElements == null) {
            availableElements = new ElementsEnum[elementsSpawnChance.Count];
            for (int i = 0; i < availableElements.Length; i++) {
                availableElements[i] = elementsSpawnChance[i].element;
            }
        }
        return availableElements;
    }
}