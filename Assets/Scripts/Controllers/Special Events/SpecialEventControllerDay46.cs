using UnityEngine;
using System.Collections;

public class SpecialEventControllerDay46 : MonoBehaviour {
    private const float DEFAULT_SPAWN_OFFSET_X = 6f;

    private float duration;

    Coroutine generation;

    [SerializeField]
    GameObject[] formationPrefabs;

    void Start() {
        // Fill duration
        duration = StageController.controller.GetCurrentMomentDuration();

        // Spawn genesis asteroid farther from the player
        generation = StartCoroutine(GenerateFormations());
    }

    private void FixedUpdate() {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) {
            StopCoroutine(generation);
            Destroy(gameObject);
        }
    }

    IEnumerator GenerateFormations() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));
            // Choose one of available formations
            int chosenIndex = Random.Range(0, formationPrefabs.Length);

            Formation newFormation = GameObject.Instantiate(formationPrefabs[chosenIndex],
               new Vector3(GameController.GetCameraXMax() + ForegroundController.SPAWN_CAMERA_OFFSET, Random.Range(GameController.GetCameraYMin() / 2, GameController.GetCameraYMax() / 2), 0),
               Quaternion.identity).GetComponent<Formation>();
            Formation.ElementsAmount amount = GameController.RollChance(50f) ? Formation.ElementsAmount.Medium : Formation.ElementsAmount.Low;
            newFormation.SetAmount(amount);
            newFormation.SetElementTypes(new ElementsEnum[] { ElementsEnum.POSITIVE_ENERGY, ElementsEnum.NEGATIVE_ENERGY });
        }
    }

}