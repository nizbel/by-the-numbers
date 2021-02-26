using UnityEngine;
using System.Collections;

public class SpecialEventControllerDay22 : MonoBehaviour {
    private const float DEFAULT_SPAWN_OFFSET_X = 2;
    private const float SMALL_WALL_CHANCE = 40;

    float duration;

    float defaultSpawnPositionX;

    ElementsEnum energyType;

    /*
	 * Energy wall prefab
	 */
    [SerializeField]
    GameObject energyWallPrefab;

    Coroutine generation;

    // Use this for initialization
    void Start() {
        // Define if energies should be positive or negative depending on time since level load
        // (This way every usage of this special event points to the same type of energy)
        energyType = Mathf.RoundToInt(Time.time - Time.timeSinceLevelLoad) % 2 == 0 ? ElementsEnum.POSITIVE_ENERGY : ElementsEnum.NEGATIVE_ENERGY;

        // Define default spawn position
        defaultSpawnPositionX = GameController.GetCameraXMax() + DEFAULT_SPAWN_OFFSET_X;

        // Fill duration
        duration = StageController.controller.GetCurrentMomentDuration();
        SpawnWallFormation();

        generation = StartCoroutine(GenerateWalls());
    }

    private void FixedUpdate() {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) {
            StopCoroutine(generation);
            Destroy(gameObject);
        }
    }

    void SpawnWallFormation() {
        // Spawn walls composed by chosen type of energy
        WallFormation newWallFormation = GameObject.Instantiate(energyWallPrefab).GetComponent<WallFormation>();
        newWallFormation.SetElementTypes(new ElementsEnum[] { energyType });
        newWallFormation.SetType(WallFormation.SEQUENTIAL_ELEMENTS_TYPE);

        // Choose size and position
        Formation.ElementsAmount amount = DefineAmount();
        newWallFormation.SetAmount(amount);
        if (amount == Formation.ElementsAmount.Medium) {
            // Big walls are set in the middle of the screen with a larger gap between elements
            newWallFormation.transform.position = new Vector3(defaultSpawnPositionX, Random.Range(-1.5f, 1.5f), 0);
            newWallFormation.SetDistanceType(WallFormation.ElementsDistanceType.FixedOffset);
            newWallFormation.SetDistance(2.1f);
        } else {
            // Small and medium have elements close by and are positioned randomly through the screen
            newWallFormation.transform.position = new Vector3(
                defaultSpawnPositionX, 
                (GameController.RollChance(50) ? 1 : -1) * Random.Range(GameController.GetCameraYMax()/2, GameController.GetCameraYMax()), 
                0);
            newWallFormation.SetDistanceType(WallFormation.ElementsDistanceType.RandomDistance);
            newWallFormation.SetMinDistance(1.05f);
            newWallFormation.SetMaxDistance(1.2f);
        }

    }

    IEnumerator GenerateWalls() {
        while (true) {
            yield return new WaitForSeconds(ForegroundElementGenerator.EASY_MIN_SPAWN_INTERVAL);
            SpawnWallFormation();
        }
    }

    Formation.ElementsAmount DefineAmount() {
        float chance = Random.Range(0, 100);
        if (chance < SMALL_WALL_CHANCE) {
            return Formation.ElementsAmount.Low;
        } else {
            return Formation.ElementsAmount.Medium;
        }
    }
}