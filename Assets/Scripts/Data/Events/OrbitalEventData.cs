using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Events/OrbitalEventData")]
public class OrbitalEventData : FormationEventData {
    public ElementsEnum[] elementTypes;

    public List<OrbitalFormation.OrbitFormationSpeedsEnum> availableOrbitSpeeds;

    public bool allElementsStartSameAngle = false;

    public override void FillEventWithData(GameObject newEventObject) {
        OrbitalEvent newEvent = newEventObject.GetComponent<OrbitalEvent>();

        SetBasicData(newEvent);

        newEvent.SetElementTypes(elementTypes);
        newEvent.SetAmount(amount);
        newEvent.SetOrbitSpeeds(availableOrbitSpeeds[Random.Range(0, availableOrbitSpeeds.Count)]);
        newEvent.SetAllElementsStartSameAngle(allElementsStartSameAngle);
    }
}