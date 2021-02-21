using UnityEngine;

public class SpecialEventMagneticBarrierController : ElementSpecialEventController {
    // Unique states for magnetic barrier event
    private const int STARTING = 1;
    private const int WAITING = 2;
    private const int INSIDE_AREA = 3;
    private const int LEAVING_AREA = 4;
    private const int PAST_BARRIERS = 5;

    void Start() {
        // Define element and speeches
        elementType = ElementsEnum.MAGNETIC_BARRIER;

        // Increases stage moment's duration to last as much as necessary
        stageMoment.duration = 60;

        state = STARTING;
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
                        ObserveElement();
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
                        observableElement = SpawnMagneticBarrier(false);
                    }
                } // Left area
                else if (!observableElement.activeSelf && NarratorController.controller.GetState() != NarratorController.IMPORTANT) {
                    SpeakAboutElement();
                    state = PAST_BARRIERS;
                }
                break;

            case PAST_BARRIERS:
                if (NarratorController.controller.GetState() != NarratorController.IMPORTANT) {
                    // End moment
                    EndStageMoment();

                    // Save element as seen
                    UpdateElementsSeen();

                    Destroy(gameObject);
                }
                break;

        }

    }

    GameObject SpawnMagneticBarrier(bool positive) {
        GameObject newMagneticBarrier = ObjectPool.SharedInstance.SpawnPooledObject(ElementsEnum.MAGNETIC_BARRIER, new Vector3(GameController.GetCameraXMax() + 2, 0, 0), Quaternion.identity);

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