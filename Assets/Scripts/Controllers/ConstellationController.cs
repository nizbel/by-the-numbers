using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstellationController : MonoBehaviour
{
    public static readonly int[] AVAILABLE_CONSTELLATIONS = { 1, 2 };

    [SerializeField]
    Constellation[] constellations;

    Constellation constellation = null;

    [SerializeField]
    GameObject starShinePrefab;

    [SerializeField]
    GameObject newConstellationInfoPrefab;

    [SerializeField]
    StageMoment constellationObservingMoment;

    public static ConstellationController controller;

    void Awake() {
        if (controller == null) {
            controller = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ChooseConstellationRandomly();

        constellation.Form();

        // Add an empty stage moment at the end of the day to show the constellation
        (StageController.controller as StoryStageController).AddEndingStageMoment(constellationObservingMoment);
    }

    // Observe the constellation, adding info to the player save data
    // Returns whether the constellation is new or not
    public bool ObserveConstellation() {
        // Identify constellation
        int constellationId = constellation.GetId();

        // Gets current constellation info from GameController
        ConstellationInfo constellationInfo = GameController.GetGameInfo().GetConstellationInfoById(constellationId);

        // Checks if it is a new constellation
        if (constellationInfo == null) {
            // New constellation
            ConstellationInfo newConstellationInfo = new ConstellationInfo();
            newConstellationInfo.timesSeen = 1;
            newConstellationInfo.id = constellationId;
            GameController.GetGameInfo().AddConstellationInfo(newConstellationInfo);

            // TODO Warn player about finding it
            StartCoroutine(InfoNewConstellationFound());
            return true;
        } else {
            // Increase times seen in the constellation
            constellationInfo.timesSeen += 1;
            GameController.GetGameInfo().UpdateConstellationInfo(constellationInfo);
            return false;
        }
    }

    public bool NewConstellation() {
        return GameController.GetGameInfo().GetConstellationInfoById(constellation.GetId()) == null;
    }

    void ChooseConstellationRandomly() {
        // Prepare spawning chances
        float[] spawningChances = new float[constellations.Length];
        float currentChance = 0;

        // Iterate through constellations found to decrease their spawning chance
        ConstellationInfo[] constellationInfo = GameController.GetGameInfo().constellationInfo;

        for (int i = 0; i < constellations.Length; i++) {
            Constellation currentConstellation = constellations[i];
            foreach (ConstellationInfo info in constellationInfo) {
                if (info == null) {
                    break;
                }
                if (currentConstellation.GetId() == info.id) {
                    currentConstellation.SetSpawnChance(Constellation.NEXT_SPAWN_CHANCE);
                    break;
                }
            }
            currentChance += currentConstellation.GetSpawnChance();
            spawningChances[i] = currentChance;
        }


        // Choose constellation at random
        float randomChoice = UnityEngine.Random.Range(0, spawningChances[constellations.Length - 1]);
        for (int i = 0; i < spawningChances.Length; i++) {
            if (spawningChances[i] > randomChoice) {
                constellation = constellations[i];
                break;
            }
        }
        // TODO Remove forcing code to point at specific constellation
        //constellation = constellations[2];
    }

    IEnumerator InfoNewConstellationFound() {
        yield return new WaitForSeconds(1.5f);

        ShineConstellation();

        GameObject.Instantiate(newConstellationInfoPrefab);

        // TODO Play new constellation found sound
    }

    void ShineConstellation() {
        float maxDelay = 0.4f;
        float startDelay = 0.2f;
        float currentDelay = startDelay;
        foreach (Star star in constellation.GetStars()) {
            ParticleSystem.MainModule mainModule = GameObject.Instantiate(starShinePrefab, star.transform).GetComponent<ParticleSystem>().main;
            mainModule.startDelay = currentDelay;
            currentDelay += (maxDelay - startDelay) / constellation.GetStars().Count;
        }
    }

    /*
     * Getters and Setters
     */
    public Constellation GetConstellation() {
        return constellation;
    }
}
