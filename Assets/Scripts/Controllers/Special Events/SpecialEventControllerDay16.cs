using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class SpecialEventControllerDay16 : MonoBehaviour {

    private const int SPAWN_WALL_1 = 1;
    private const int SPAWN_WALL_2 = 2;
    private const int SPAWN_MAGNETIC_BARRIER_1 = 3;
    private const int SPAWN_MAGNETIC_BARRIER_2 = 4;

    private float waitTime = 1;

    private int state = SPAWN_WALL_1;

    /*
	 * Prefabs
	 */
    public GameObject energyWallPrefab;

    // Keeps track of whether it's the second time a magnetic barrier spawns
    bool secondRound = false;

    // Update is called once per frame
    void Update() {
        if (waitTime > 0) {
            waitTime -= Time.deltaTime;
        } else {
            switch (state) {
                case SPAWN_WALL_1:
                    WarnAboutMagneticBarrier(true);
                    SpawnWall(true);
                    state = SPAWN_WALL_2;
                    break;

                case SPAWN_WALL_2:
                    SpawnWall(true);
                    if (secondRound) {
                        state = SPAWN_MAGNETIC_BARRIER_2;
                    }
                    else {
                        state = SPAWN_MAGNETIC_BARRIER_1;
                    }
                    break;

                case SPAWN_MAGNETIC_BARRIER_1:
                    SpawnMagneticBarrier(true);
                    state = SPAWN_WALL_1;
                    secondRound = true;
                    break;

                case SPAWN_MAGNETIC_BARRIER_2:
                    SpawnMagneticBarrier(true);
                    Destroy(gameObject);
                    break;
            }
            waitTime = 1;
        }
    }


    void SpawnWall(bool positive) {
        WallFormation energyWall = Instantiate(energyWallPrefab, new Vector3(GameController.GetCameraXMax() + 2, 0, 0),
                                                                      transform.rotation).GetComponent<WallFormation>();
        // Set type and size
        energyWall.SetDistance(1.3f);
        energyWall.SetAmount(WallFormation.MAX_AMOUNT);
        if (positive) {
            energyWall.SetElementTypes(new ElementsEnum[] { ElementsEnum.POSITIVE_ENERGY });
        } else {
            energyWall.SetElementTypes(new ElementsEnum[] { ElementsEnum.NEGATIVE_ENERGY });
        }
    }

    void SpawnMagneticBarrier(bool positive) {
        GameObject newMagneticBarrier = ObjectPool.SharedInstance.SpawnPooledObject(ElementsEnum.MAGNETIC_BARRIER, new Vector3(GameController.GetCameraXMax() + 2, 0, 0), Quaternion.identity);

        // Set whether it is positive
        newMagneticBarrier.GetComponent<MagneticBarrier>().SetPositive(positive);
    }


    // Show warning regarding magnetic barrier
    void WarnAboutMagneticBarrier(bool positive) {
        ValueRange.controller.ActivateMagneticBarrierWarning(positive);
    }
}