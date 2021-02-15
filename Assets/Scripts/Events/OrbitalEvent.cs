using UnityEngine;

/**
 * Creates elements that travel towards the player in an orbit
 */
public class OrbitalEvent : FormationEvent {
	ElementsEnum[] elementTypes;

    OrbitalFormation.OrbitFormationSpeedsEnum orbitSpeeds;

    bool allElementsStartSameAngle;

    public void SetElementTypes(ElementsEnum[] elementTypes) {
        this.elementTypes = elementTypes;
    }

    public void SetOrbitSpeeds(OrbitalFormation.OrbitFormationSpeedsEnum orbitSpeeds) {
        this.orbitSpeeds = orbitSpeeds;
    }

    public void SetAllElementsStartSameAngle(bool allElementsStartSameAngle) {
        this.allElementsStartSameAngle = allElementsStartSameAngle;
    }

    protected override void StartEvent() {
        // Choose one of available formations
        int chosenIndex = Random.Range(0, availableFormations.Length);

        // Generate formation
        OrbitalFormation newFormation = GameObject.Instantiate(availableFormations[chosenIndex],
            new Vector3(GameController.GetCameraXMax() + ForegroundController.SPAWN_CAMERA_OFFSET, Random.Range(GameController.GetCameraYMin() / 2, GameController.GetCameraYMax() / 2), 0),
            Quaternion.identity).GetComponent<OrbitalFormation>();
        newFormation.SetAmount(amount);
        newFormation.SetElementTypes(elementTypes);
        newFormation.SetOrbitSpeedsType(orbitSpeeds);
        newFormation.SetAllElementsStartSameAngle(allElementsStartSameAngle);

        // Disappear
        Destroy(gameObject);
    }
}
