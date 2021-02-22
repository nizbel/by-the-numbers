using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovingFormationsEvent : ForegroundEvent {

    [SerializeField]
    protected GameObject[] availableFormations;

    [Serializable]
    public class FormationData {
        public Formation.ElementsAmount amount;

        public Vector3 speed;

        public Vector3 position;
    }

    protected FormationData[] formations;

    public void SetFormations(FormationData[] formations) {
        this.formations = formations;
    }

    protected override void StartEvent() {
        // Choose one of available formations
        int chosenIndex = Random.Range(0, availableFormations.Length);

        // Generate formations
        foreach (FormationData formationData in formations) {
            Formation newFormation = GameObject.Instantiate(availableFormations[chosenIndex],
                formationData.position,
                Quaternion.identity).GetComponent<Formation>();
            newFormation.SetAmount(formationData.amount);

            // Speed
            IMovingObject movingScript = newFormation.gameObject.GetComponent<IMovingObject>();
            movingScript.SetSpeed(formationData.speed);
        }

        // Disappear
        Destroy(gameObject);
    }
}
