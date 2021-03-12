using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Events/FormationEventData")]
public class FormationEventData : EventData {

    [Header("Specific attributes")]
    public Formation.ElementsAmount amount;

    public override void FillEventWithData(GameObject newEventObject) {
        FormationEvent newEvent = newEventObject.GetComponent<FormationEvent>();

        SetBasicData(newEvent);

        newEvent.SetAmount(amount);
    }
}