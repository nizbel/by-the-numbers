using System;
using System.Collections.Generic;
using UnityEngine;

public class ConstellationController : MonoBehaviour
{
    private readonly int[] DAYS_WITHOUT_CONSTELLATIONS = { 1, 32, 90 };

    [SerializeField]
    Constellation[] constellations;

    Constellation constellation = null;

    public static ConstellationController controller;

    void Awake() {
        if (controller == null) {
            controller = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Check if current day is allowed to spawn constellation
        if (!CurrentDayCanSpawnConstellation()) {
            controller = null;
            Destroy(this);
        }

        // Check constellations user hasn't found yet
        List<Constellation> constellationsNotFound = new List<Constellation>();

        // TODO Ask game controller what are the constellations yet to be found
        constellationsNotFound.AddRange(constellations);

        // Choose constellation at random
        constellation = constellationsNotFound[UnityEngine.Random.Range(0, constellations.Length)];
        constellation.Form();
    }

    bool CurrentDayCanSpawnConstellation() {
        int currentDay = GameController.controller.GetCurrentDay();
        return !Array.Exists(DAYS_WITHOUT_CONSTELLATIONS, element => element == currentDay);
    }

    /*
     * Getters and Setters
     */
    public Constellation GetConstellation() {
        return constellation;
    }
}
