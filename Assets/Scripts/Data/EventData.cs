using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Events/EventData")]
public class EventData : ScriptableObject {
    public GameObject foregroundEvent;

    public DifficultyEnum difficulty;

    public List<ElementsEnum> obligatoryElements;

    public List<ElementsEnum> optionalElements;

    [Min(1f)]
    public float duration = 1;

    [Min(0)]
    public int chargesCost = 0;

    [Min(0f)]
    public float cooldown = 0;

    [Min(0f)]
    public float delay = 0;

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