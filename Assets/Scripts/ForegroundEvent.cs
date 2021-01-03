using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundEvent : MonoBehaviour
{
    [SerializeField]
    int chargesCost = 1;

    [SerializeField]
    float cooldown = 0;

    // Start is called before the first frame update
    void Start()
    {
        
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
}
