using UnityEngine;
using System.Collections;

public class SpecialEventControllerDay17 : MonoBehaviour {
    private const float DEFAULT_SPAWN_OFFSET_X = -2;
    private const float DEFAULT_SPAWN_OFFSET_Y = 10;
    private const float DEFAULT_ELEMENTS_DISTANCE = 2f;
    private const float WAIT_TIME_FOR_WALL = 1f;

    float duration;

    /*
	 * Energy wall prefab
	 */
    [SerializeField]
    GameObject energyWallPrefab;

    // Use this for initialization
    void Start() {
        // Fill duration
        duration = StageController.controller.GetCurrentMomentDuration();

        // Generates the wall that will pass through the stage moment
        GenerateWall();
    }

    // Update is called once per frame
    void FixedUpdate() {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) {
            Destroy(gameObject);
        }
    }


    void GenerateWall() {
        // Spawn walls composed by random energies
        WallFormation newWallFormation = GameObject.Instantiate(energyWallPrefab).GetComponent<WallFormation>();
        newWallFormation.SetElementTypes(new ElementsEnum[] { ElementsEnum.POSITIVE_ENERGY, ElementsEnum.NEGATIVE_ENERGY });
        newWallFormation.SetNonDestructibleAtStart(true);


        // Set position initially to out of screen and distance between elements
        newWallFormation.transform.position = new Vector3(GameController.GetCameraXMin() + DEFAULT_SPAWN_OFFSET_X, GameController.GetCameraYMax() + DEFAULT_SPAWN_OFFSET_Y, 0);
        newWallFormation.SetAmount(Formation.ElementsAmount.High);
        newWallFormation.SetType(WallFormation.RANDOM_ELEMENTS_TYPE);
        newWallFormation.SetDistanceType(WallFormation.ElementsDistanceType.FixedOffset);
        newWallFormation.SetDistance(DEFAULT_ELEMENTS_DISTANCE);

        StartCoroutine(SetWallTrajectory(newWallFormation));

    }

    // Apply trajectory to wall
    IEnumerator SetWallTrajectory(WallFormation wallFormation) {
        yield return new WaitForSeconds(WAIT_TIME_FOR_WALL);

        // Get the highest and the lowest bounds of the wall
        Transform highestElement = wallFormation.GetCenterElement();
        Transform lowestElement = wallFormation.GetCenterElement();
        foreach (Transform child in wallFormation.transform) {
            if (child.position.y < lowestElement.position.y) {
                lowestElement = child;
            } else if (child.position.y > highestElement.position.y) {
                highestElement = child;
            }
        }

        // Set position correctly now considering the walls' size
        Vector3 targetPosition;
        if (GameController.RollChance(50)) {
            // Start at top
            wallFormation.transform.position = new Vector3(GameController.GetCameraXMin(),
                GameController.GetCameraYMax() + (wallFormation.transform.position.y - lowestElement.position.y) + GameObjectUtil.GetGameObjectHalfVerticalSize(lowestElement.gameObject),
                0);

            targetPosition = new Vector3(GameController.GetCameraYMax() - DEFAULT_SPAWN_OFFSET_X,
                GameController.GetCameraYMin() - (highestElement.position.y - wallFormation.transform.position.y),
                0);
        } else {
            // Start at bottom
            wallFormation.transform.position = new Vector3(GameController.GetCameraXMin(),
                GameController.GetCameraYMin() - (highestElement.position.y - wallFormation.transform.position.y) - GameObjectUtil.GetGameObjectHalfVerticalSize(highestElement.gameObject),
                0);

            targetPosition = new Vector3(GameController.GetCameraYMax() - DEFAULT_SPAWN_OFFSET_X,
                GameController.GetCameraYMax() + (wallFormation.transform.position.y - lowestElement.position.y),
                0);
        }

        // Apply speed considering it will cross through the entire duration
        wallFormation.gameObject.GetComponent<IMovingObject>().SetSpeed(PlayerController.controller.GetSpeed() * Vector3.right + 
            (targetPosition - wallFormation.transform.position) / (duration));
    }

}