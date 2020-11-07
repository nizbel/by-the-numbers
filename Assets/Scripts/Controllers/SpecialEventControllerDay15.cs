using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class SpecialEventControllerDay15 : MonoBehaviour {

    private const int SPAWN_WALL_1 = 1;
    private const int SPAWN_WALL_2 = 2;
    private const int SPAWN_RANGE_CHANGER_1 = 3;
    private const int SPAWN_RANGE_CHANGER_2 = 4;

    private int currentDay;

    private int eventCode;

    public int CurrentDay { get => currentDay; set => currentDay = value; }
    public int EventCode { get => eventCode; set => eventCode = value; }

    private float waitTime = 1;

    private int state = SPAWN_WALL_1;

    /*
	 * Prefabs
	 */
    public GameObject rangeChangerPrefab;
    public GameObject rangeChangeWarningPrefab;
    public GameObject energyWallPrefab;

    // Keeps track of whether it's the second time a range changer spawns
    bool secondRound = false;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (waitTime > 0) {
            waitTime -= Time.deltaTime;
        } else {
            switch (state) {
                case SPAWN_WALL_1:
                    WarnAboutRangeChanger(true);
                    SpawnWall(true);
                    state = SPAWN_WALL_2;
                    break;

                case SPAWN_WALL_2:
                    SpawnWall(true);
                    if (secondRound) {
                        state = SPAWN_RANGE_CHANGER_2;
                    }
                    else {
                        state = SPAWN_RANGE_CHANGER_1;
                    }
                    break;

                case SPAWN_RANGE_CHANGER_1:
                    SpawnRangeChanger(true);
                    state = SPAWN_WALL_1;
                    secondRound = true;
                    break;

                case SPAWN_RANGE_CHANGER_2:
                    SpawnRangeChanger(true);
                    Destroy(gameObject);
                    break;
            }
            waitTime = 1;
        }
    }


    void SpawnWall(bool positive) {
        GameObject energyWall = (GameObject)Instantiate(energyWallPrefab, new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x + 2, 0, 0),
                                                                      transform.rotation);
        // Set type and size
        energyWall.GetComponent<EnergyWall>().SetDistance(1.3f);
        energyWall.GetComponent<EnergyWall>().SetSize(EnergyWall.MAX_SIZE);
        if (positive) {
            energyWall.GetComponent<EnergyWall>().SetType(EnergyWall.POSITIVE_ENERGIES);
        } else {
            energyWall.GetComponent<EnergyWall>().SetType(EnergyWall.NEGATIVE_ENERGIES);
        }
    }

    void SpawnRangeChanger(bool positive) {
        GameObject newRangeChanger = (GameObject)Instantiate(rangeChangerPrefab, new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x + 2, 0, 0),
                                                                      transform.rotation);
        // Set whether it is positive
        newRangeChanger.GetComponent<RangeChanger>().SetPositive(positive);
    }


    // Show warning regarding range changer
    void WarnAboutRangeChanger(bool positive) {
        GameObject rangeChangerWarning = GameObject.Instantiate(rangeChangeWarningPrefab);
        if (positive) {
            rangeChangerWarning.GetComponent<Light2D>().color = new Color(0.05f, 0.05f, 0.92f);
        }
        else {
            rangeChangerWarning.GetComponent<Light2D>().color = new Color(0.92f, 0.05f, 0.05f);
        }
    }
}