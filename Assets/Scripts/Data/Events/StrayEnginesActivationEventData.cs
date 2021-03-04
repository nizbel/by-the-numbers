using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Events/StrayEnginesActivationEventData")]
public class StrayEnginesActivationEventData : EventData {

    public StrayEnginesActivationEvent.AmountEnum amount;

    public override void FillEventWithData(GameObject newEventObject) {
        StrayEnginesActivationEvent newEvent = newEventObject.GetComponent<StrayEnginesActivationEvent>();

        SetBasicData(newEvent);

        newEvent.SetAmount(amount);
    }
}