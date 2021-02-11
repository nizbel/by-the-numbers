using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationEvent : ForegroundEvent {
    

    [SerializeField]
    GameObject[] availableFormations;

    Formation.ElementsAmount amount;

    public void SetAmount(Formation.ElementsAmount amount) {
        this.amount = amount;
    }

    protected override void StartEvent() {
        // Choose one of available formations
        int chosenIndex = Random.Range(0, availableFormations.Length);

        // Generate formation
        Formation newFormation = GameObject.Instantiate(availableFormations[chosenIndex], 
            new Vector3(GameController.GetCameraXMax() + ForegroundController.SPAWN_CAMERA_OFFSET, Random.Range(GameController.GetCameraYMin()/2, GameController.GetCameraYMax()/2), 0), 
            Quaternion.identity).GetComponent<Formation>();
        newFormation.SetAmount(amount);

        // Disappear
        Destroy(gameObject);
    }
}
