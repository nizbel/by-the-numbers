using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Events/ShowerEventData")]
public class ShowerEventData : EventData {
    public ElementsEnum elementType;

    public ShowerEvent.Duration showerDuration;

    public MeteorGenerator.Intensity intensity;

    public override void FillEventWithData(GameObject newEventObject) {
        ShowerEvent newEvent = newEventObject.GetComponent<ShowerEvent>();

        SetBasicData(newEvent);

        newEvent.SetElementType(elementType);
        newEvent.SetShowerDuration(showerDuration);
        newEvent.SetIntensity(intensity);
    }
}