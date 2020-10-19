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

    /*
	 * Block prefabs
	 */
    public GameObject rangeChangerPrefab;
    public GameObject rangeChangeWarningPrefab;

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
                        WarnAboutRangeChanger(true);
                        state = WAITING;
                        waitTime = 2;
                    }
                }
                break;

            case WAITING:
                if (waitTime > 0) {
                    waitTime -= Time.deltaTime;
                    if (waitTime <= 0) {
                        GameObject initialRangeChanger = SpawnRangeChanger(true);
                        initialRangeChanger.GetComponent<RangeChanger>().AddPastListener(PastThroughInitialRangeChanger);
                    }
                }
                break;

            case INSIDE_AREA:
                if (waitTime > 0) {
                    waitTime -= Time.deltaTime;
                    if (waitTime <= 0) {
                        WarnAboutRangeChanger(false);
                        state = LEAVING_AREA;
                        waitTime = 2;
                    }
                }
                break;

            case LEAVING_AREA:
                if (waitTime > 0) {
                    waitTime -= Time.deltaTime;
                    if (waitTime <= 0) {
                        GameObject finalRangeChanger = SpawnRangeChanger(false);
                        Destroy(gameObject);
                    }
                }
                break;
        }

    }

    GameObject SpawnRangeChanger(bool positive) {
        GameObject newRangeChanger = (GameObject)Instantiate(rangeChangerPrefab, new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x + 2, 0, 0),
                                                                      transform.rotation);
        // Set whether it is positive
        newRangeChanger.GetComponent<RangeChanger>().SetPositive(positive);

        return newRangeChanger;
    }


    // Show warning regarding range changer
    void WarnAboutRangeChanger(bool positive) {
        GameObject rangeChangerWarning = GameObject.Instantiate(rangeChangeWarningPrefab);
        if (positive) {
            rangeChangerWarning.GetComponent<Light2D>().color = new Color(0.05f, 0.05f, 0.92f);
            NarratorController.controller.StartEventSpeech("Day 5 - What is that");
        }
        else {
            rangeChangerWarning.GetComponent<Light2D>().color = new Color(0.92f, 0.05f, 0.05f);
        }
    }

    void PastThroughInitialRangeChanger() {
        state = INSIDE_AREA;
        waitTime = 1;
    }
}