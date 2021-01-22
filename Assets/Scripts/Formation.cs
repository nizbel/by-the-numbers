using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation : ForegroundEvent {
    [SerializeField]
    protected float screenOffset = 0;

    void Awake() {
        // Update cooldown
        SetCooldown(0.1f * transform.childCount);
    }

    public virtual float GetScreenOffset() {
        return screenOffset;
    }
}
