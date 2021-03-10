using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Events/ElementCorridorEventData")]
public class ElementCorridorEventData : EventData
{
    public float amount;

    public override void FillEventWithData(GameObject newEventObject) {
        ElementCorridorEvent newEvent = newEventObject.GetComponent<ElementCorridorEvent>();

        SetBasicData(newEvent);

        List<ElementsEnum> elementsAvailable = new List<ElementsEnum>();
        elementsAvailable.AddRange(obligatoryElements);
        elementsAvailable.AddRange(optionalElements);
        newEvent.SetElementsAvailable(elementsAvailable.ToArray());
        newEvent.SetAmount(amount);
    }
}
