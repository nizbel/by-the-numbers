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
        }
        Destroy(this);
    }

    public void SetActivatingChance(float activatingChance) {
        this.activatingChance = activatingChance;
    }

}
