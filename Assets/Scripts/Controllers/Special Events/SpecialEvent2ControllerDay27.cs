using UnityEngine;
using System.Collections;

public class SpecialEvent2ControllerDay27: MonoBehaviour {
    private const float DEFAULT_SPAWN_OFFSET_X = 2;

    float duration;

    float defaultSpawnPositionX;

    /*
	 * Energy wall prefab
	 */
    [SerializeField]
    GameObject wallPrefab;

    Coroutine generation;

    // Use this for initialization
    void Start() {
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
        WallFormation newWallFormation = GameObject.Instantiate(wallPrefab).GetComponent<WallFormation>();
        newWallFormation.SetElementTypes(new ElementsEnum[] { ElementsEnum.DEBRIS });
        newWallFormation.SetType(WallFormation.SEQUENTIAL_ELEMENTS_TYPE);

        // Choose size and position
        //newWallFormation.SetAmount(Formation.ElementsAmount.High);
        newWallFormation.SetAmount(WallFormation.MAX_AMOUNT * Random.Range(2, 3));
        // Create dense wall
        newWallFormation.transform.position = new Vector3(
            defaultSpawnPositionX,
            Random.Range(-1f, 1f),
            0);
        newWallFormation.SetDistanceType(WallFormation.ElementsDistanceType.RandomDistance);
        newWallFormation.SetMinDistance(0.6f);
        newWallFormation.SetMaxDistance(0.8f);

        StartCoroutine(GenerateEnergies());
    }

    void SpawnEnergiesIntoWall() {
        // Spawn energies that dissolves debris in walls
        ElementsEnum energyType = GameController.RollChance(50) ? ElementsEnum.POSITIVE_ENERGY : ElementsEnum.NEGATIVE_ENERGY;

        Vector3 energyPosition = new Vector3(
            GameController.GetCameraXMin() - Random.Range(0, 1f), 
            (GameController.GetCameraYMax() + 2) * (GameController.RollChance(50) ? 1 : -1), 
            0);
        GameObject newEnergy = ObjectPool.SharedInstance.SpawnPooledObject(energyType, energyPosition, Quaternion.identity);

        newEnergy.GetComponent<IMovingObject>().SetSpeed(new Vector3(PlayerController.controller.GetSpeed() * 2f, -energyPosition.y * Random.Range(0.75f, 1.25f), 0));

        // Chance to spawn another energy
        if (GameController.RollChance(33.33f)) {
            StartCoroutine(GenerateEnergies());
        }
    }

    IEnumerator GenerateWalls() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(ForegroundElementGenerator.EASY_MIN_SPAWN_INTERVAL, ForegroundElementGenerator.EASY_MAX_SPAWN_INTERVAL));
            SpawnWallFormation();
        }
    }

    IEnumerator GenerateEnergies() {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
        SpawnEnergiesIntoWall();
    }
}