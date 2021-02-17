using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Events/BackElementEventData")]
public class BackElementEventData : EventData
{
    public BackElementGenerator.AmountEnum amount;

    public override void FillEventWithData(GameObject newEventObject) {
        BackElementEvent newEvent = newEventObject.GetComponent<BackElementEvent>();

        SetBasicData(newEvent);

        List<ElementsEnum> elementsAvailable = new List<ElementsEnum>();
        elementsAvailable.AddRange(obligatoryElements);
        elementsAvailable.AddRange(optionalElements);
        newEvent.SetElementsAvailable(elementsAvailable.ToArray());
        newEvent.SetAmount(amount);
    }
}
