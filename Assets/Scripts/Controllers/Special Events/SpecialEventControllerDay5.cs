using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Rendering.Universal;

public class SpecialEventControllerDay5 : MonoBehaviour {

    private const int STARTING = 1;
    private const int WAITING = 2;
    private const int INSIDE_AREA = 3;
    private const int LEAVING_AREA = 4;

    private int currentDay;

    private int eventCode;

    public int CurrentDay { get => currentDay; set => currentDay = value; }
    public int EventCode { get => eventCode; set => eventCode = value; }

    private float waitTime = 1;

    private int state = STARTING;

    private float randomOffset = 0;

    // Use this for initialization
    void Start() {
        randomOffset = Random.Range(0,1f);
    }

    // Update is called once per frame
    void Update() {
        switch (state) {
            case STARTING:
                if (waitTime > 0) {
                    waitTime -= Time.deltaTime;
                    if (waitTime <= 0) {
                        WarnAboutMagneticBarrier(true);
                        state = WAITING;
                        waitTime = 2;
                    }
                }
                break;

            case WAITING:
                if (waitTime > 0) {
                    waitTime -= Time.deltaTime;
                    if (waitTime <= 0) {
                        GameObject initialMagneticBarrier = SpawnMagneticBarrier(true);
                        initialMagneticBarrier.GetComponent<MagneticBarrier>().AddPastListener(PastThroughInitialMagneticBarrier);
                    }
                }
                break;

            case INSIDE_AREA:
                if (waitTime > 0) {
                    waitTime -= Time.deltaTime;
                    if (waitTime <= 0) {
                        WarnAboutMagneticBarrier(false);
                        state = LEAVING_AREA;
                        waitTime = 2;
                    }
                }
                break;

            case LEAVING_AREA:
                if (waitTime > 0) {
                    waitTime -= Time.deltaTime;
                    if (waitTime <= 0) {
                        GameObject finalMagneticBarrier = SpawnMagneticBarrier(false);
                        Destroy(gameObject);
                    }
                }
                break;
        }

    }

    GameObject SpawnMagneticBarrier(bool positive) {
        GameObject newMagneticBarrier = ObjectPool.SharedInstance.SpawnPooledObject(ObjectPool.MAGNETIC_BARRIER, new Vector3(GameController.GetCameraXMax() + 2, 0, 0), Quaternion.identity);

        // Set whether it is positive
        newMagneticBarrier.GetComponent<MagneticBarrier>().SetPositive(positive);

        return newMagneticBarrier;
    }


    // Show warning regarding magnetic barrier
    void WarnAboutMagneticBarrier(bool positive) {
        ValueRange.controller.ActivateMagneticBarrierWarning(positive);
        if (positive) {
            // TODO Find a way to unify magnetic barrier warning energy definition
            //NarratorController.controller.StartMomentSpeech("Day 5 - What is that");
        }
    }

    void PastThroughInitialMagneticBarrier() {
        state = INSIDE_AREA;
        waitTime = 1;
    }
}