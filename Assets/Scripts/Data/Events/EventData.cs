using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Events/EventData")]
public class EventData : ScriptableObject {
    public GameObject foregroundEvent;

    public DifficultyEnum difficulty;

    [Tooltip("Elements that need to be present for the event to spawn")]
    public List<ElementsEnum> obligatoryElements;

    [Tooltip("Elements that can appear, as long as at least one of them is available")]
    public List<ElementsEnum> optionalElements;

    [Tooltip("Time that should be allocated for the event to spawn")]
    [Min(1f)]
    public float duration = 1;

    [Tooltip("Cost in charges for the day")]
    [Min(0)]
    public int chargesCost = 0;

    [Tooltip("Time after spawning before element generator can resume its spawns")]
    [Min(0f)]
    public float cooldown = 0;

    [Tooltip("Time before spawning in which element generator should pause")]
    [Min(0f)]
    public float delay = 0;

    [Tooltip("Day from which this event should start showing up")]
    [Min(1f)]
    public int firstAppearingDay = 1;

    public virtual void FillEventWithData(GameObject newEventObject) {
        ForegroundEvent newEvent = newEventObject.GetComponent<ForegroundEvent>();
        SetBasicData(newEvent);
    }

    protected void SetBasicData(ForegroundEvent newEvent) {
        newEvent.SetChargesCost(chargesCost);
        newEvent.SetCooldown(cooldown);
        newEvent.SetDelay(delay);
    }
}