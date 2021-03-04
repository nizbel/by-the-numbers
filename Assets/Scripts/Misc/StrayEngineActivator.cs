using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO rethink about the usage of this class
public class StrayEngineActivator : MonoBehaviour
{
    float activatingChance = StrayEngine.DEFAULT_ACTIVATING_CHANCE;

    // Activates on start if it should activate
    void Start()
    {
        if (GameController.RollChance(activatingChance)) {
            GetComponent<StrayEngine>().Activate();
        } else {
            // Sets value as 0 for it is an inactive engine
            GetComponent<StrayEngine>().SetValue(0);
        }
        Destroy(this);
    }

    public void SetActivatingChance(float activatingChance) {
        this.activatingChance = activatingChance;
    }

}
