using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Events/OrbitalEventData")]
public class OrbitalEventData : FormationEventData {
    public ElementsEnum[] elementTypes;

    public OrbitalFormation.OrbitFormationSpeedsEnum orbitSpeeds;

    public override void FillEventWithData(GameObject newEventObject) {
        OrbitalEvent newEvent = newEventObject.GetComponent<OrbitalEvent>();

        SetBasicData(newEvent);

        newEvent.SetElementTypes(elementTypes);
        newEvent.SetAmount(amount);
    }
}