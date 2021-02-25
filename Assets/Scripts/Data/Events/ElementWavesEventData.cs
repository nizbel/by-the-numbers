using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Events/ElementWaveEventData")]
public class ElementWavesEventData : EventData {
    public ElementWavesEvent.ElementsWaveData[] elementWaves;

    public override void FillEventWithData(GameObject newEventObject) {
        ElementWavesEvent newEvent = newEventObject.GetComponent<ElementWavesEvent>();

        SetBasicData(newEvent);

        newEvent.SetElementWaves(elementWaves);
    }
}