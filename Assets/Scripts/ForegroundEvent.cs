using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundEvent : MonoBehaviour
{
    int chargesCost;

    float cooldown;

    float delay;

    protected void Start() {
        ForegroundController.controller.EventSpawned(this);
        if (delay > 0) {
            StartCoroutine(StartEventDelayed());
        } else {
            StartEvent();
        }
    }

    protected IEnumerator StartEventDelayed() {
        yield return new WaitForSeconds(delay);
        StartEvent();
    }

    protected virtual void StartEvent() {

    }

    /*
     * Getters and Setters
     */
    public int GetChargesCost() {
        return chargesCost;
    }

    public void SetChargesCost(int chargesCost) {
        this.chargesCost = chargesCost;
    }

    public float GetCooldown() {
        return cooldown;
    }
    public void SetCooldown(float cooldown) {
        this.cooldown = cooldown;
    }

    public float GetDelay() {
        return delay;
    }
    public void SetDelay(float delay) {
        this.delay = delay;
    }
}
