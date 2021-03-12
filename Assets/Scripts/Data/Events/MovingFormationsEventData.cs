using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Events/MovingFormationsEventData")]
public class MovingFormationsEventData : EventData {

    [Header("Specific attributes")]
    public MovingFormationsEvent.FormationData[] formations;

    public override void FillEventWithData(GameObject newEventObject) {
        MovingFormationsEvent newEvent = newEventObject.GetComponent<MovingFormationsEvent>();

        SetBasicData(newEvent);

        newEvent.SetFormations(formations);
    }
}