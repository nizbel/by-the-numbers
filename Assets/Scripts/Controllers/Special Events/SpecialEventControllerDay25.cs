using UnityEngine;
using System.Collections;

public class SpecialEventControllerDay25 : MonoBehaviour {

    private const int SPAWN_MAGNETIC_BARRIER_1 = 1;
    // Should spawn 3 times before getting to finish state
    private const int FINISH = 4;

    private int state = SPAWN_MAGNETIC_BARRIER_1;

    private bool positiveBarriers;

    private float duration;

    void Start() {
        // Fill duration
        duration = StageController.controller.GetCurrentMomentDuration();

        // Check whether barriers should be positive or negative
        positiveBarriers = PlayerController.controller.GetValue() >= 0;

        StartCoroutine(GenerateBarriers());
    }


    void SpawnMagneticBarrier(bool positive) {
        GameObject newMagneticBarrier = ObjectPool.SharedInstance.SpawnPooledObject(ElementsEnum.MAGNETIC_BARRIER, new Vector3(GameController.GetCameraXMax() + 2, 0, 0), Quaternion.identity);

        // Set whether it is positive
        newMagneticBarrier.GetComponent<MagneticBarrier>().SetPositive(positive);

        // Advance to the next state
        state++;
    }


    // Show warning regarding magnetic barrier
    void WarnAboutMagneticBarrier(bool positive) {
        ValueRange.controller.ActivateMagneticBarrierWarning(positive);
    }

    IEnumerator GenerateBarriers() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(duration / 8, duration / 6));
            WarnAboutMagneticBarrier(positiveBarriers);
            yield return new WaitForSeconds(MagneticBarrier.WARNING_PERIOD_BEFORE_MAGNETIC_BARRIER);
            SpawnMagneticBarrier(positiveBarriers);

            if (state == FINISH) {
                break;
            }
        }

        Destroy(gameObject);
    }
}