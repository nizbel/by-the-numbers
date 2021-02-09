using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NarratorController : MonoBehaviour {
    // Narrator state constants
    public const int QUIET = 0;
    public const int IMPORTANT = 1;
    public const int WARNING = 2;

    private const float TIME_TO_WARN_AGAIN = 15.5f;

    public static NarratorController controller;

    private AudioSource narrator;
    private Text subtitles;

    private bool gameRunning = false;

    private int state = QUIET;

    // Warnings
    private float lastRangeWarning = 0;
    private float lastBarrierWarning = 0;

    private Speech currentSpeech = null;

    private bool playingSubtitles = true;
    public bool PlayingSubtitles {
        get => playingSubtitles;
        set {
            playingSubtitles = value;
            if (gameRunning) {
                if (value) {
                    ResumeSubtitles();
                }
                else {
                    PauseSubtitles();
                }
            }

            // Save into player prefs
            PlayerPrefsUtil.SetBoolPref(PlayerPrefsUtil.SHOW_SUBTITLES_PREF, value);
        }
    }
    private string currentSubtitle = "";
    private float subtitleTimer = 0;
    private int subtitleIndex = 0;

    /*
     * Common speeches
     */
    [SerializeField]
    List<Speech> energyWarningSpeeches;
    [SerializeField]
    List<Speech> positiveBarrierWarningSpeeches;
    [SerializeField]
    List<Speech> negativeBarrierWarningSpeeches;


    void Awake() {
        if (controller == null) {
            controller = this;

            // Get values from player prefs
            playingSubtitles = PlayerPrefsUtil.GetBoolPref(PlayerPrefsUtil.SHOW_SUBTITLES_PREF);
        }
        else {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update() {
        if (gameRunning) {
            if (!narrator.isPlaying && state != QUIET && Time.timeScale > 0) {
                state = QUIET;
                MusicController.controller.IncreaseVolumeAfterNarrator();

                // Clear subtitle variables
                subtitles.text = "";
                currentSubtitle = "";
            }
            else if (narrator.isPlaying) {
                PlaySubtitles();

                subtitleTimer += Time.deltaTime;
            }
        }
    }

    public void StartGame() {
        // Find narrator
        narrator = GameObject.Find("Narrator").GetComponent<AudioSource>();
        // Find subtitles
        subtitles = GameObject.Find("Subtitles").GetComponent<Text>();

        gameRunning = true;

        state = QUIET;
    }

    public void StartMomentSpeech(Speech speech, bool playOnInfinite = false) {
        // Do not play speeches during infinite mode, unless explicitelly told
        if (!playOnInfinite && GameController.controller.GetState() == GameController.GAMEPLAY_INFINITE) {
            return;
        }
        if (state == IMPORTANT) {
            Debug.LogError("Important speeches can't be stopped");
        }
        state = IMPORTANT;

        currentSpeech = speech;
        // Prepare current speech timestamp in seconds
        currentSpeech.speechParts[0].CalculateTimestampInSeconds();
        Speak(currentSpeech.audio);
    }

    public void GameOver() {
        // Stop narrator speaking
        if (state != QUIET) {
            StopSpeech();
            MusicController.controller.IncreaseVolumeAfterNarrator();
        }

        // Reset attributes
        narrator = null;

        gameRunning = false;
    }

    public void WarnRange() {
        if (state == QUIET && ShouldRangeWarnAgain()) {
            state = WARNING;

            //AudioClip clip = LoadCommonSpeech("Warning - Energy Level 1");

            currentSpeech = energyWarningSpeeches[Random.Range(0, energyWarningSpeeches.Count)];
            // Prepare current speech timestamp in seconds
            currentSpeech.speechParts[0].CalculateTimestampInSeconds();
            Speak(currentSpeech.audio);

            lastRangeWarning = Time.realtimeSinceStartup;
        }
    }

    public void WarnBarrier(bool positive) {
        if (state == QUIET && ShouldBarrierWarnAgain()) {
            state = WARNING;

            if (positive) {
                currentSpeech = positiveBarrierWarningSpeeches[Random.Range(0, positiveBarrierWarningSpeeches.Count)];
            } else {
                currentSpeech = negativeBarrierWarningSpeeches[Random.Range(0, negativeBarrierWarningSpeeches.Count)];
            }
            // Prepare current speech timestamp in seconds
            currentSpeech.speechParts[0].CalculateTimestampInSeconds();
            Speak(currentSpeech.audio);

            lastBarrierWarning = Time.realtimeSinceStartup;
        }
    }

    //private void Speak() {
    //    MusicController.controller.DecreaseVolumeForNarrator();
    //    narrator.Play();
    //}

    private void Speak(AudioClip clip) {
        MusicController.controller.DecreaseVolumeForNarrator();
        narrator.clip = clip;
        narrator.Play();

        // Reset timer and index
        subtitleTimer = 0;
        subtitleIndex = 0;
    }

    public void StopSpeech() {
        narrator.Stop();
    }

    private bool ShouldRangeWarnAgain() {
        return lastRangeWarning == 0 || (Time.realtimeSinceStartup - lastRangeWarning) > TIME_TO_WARN_AGAIN;
    }

    private bool ShouldBarrierWarnAgain() {
        return lastRangeWarning == 0 || (Time.realtimeSinceStartup - lastRangeWarning) > TIME_TO_WARN_AGAIN;
    }

    private void PlaySubtitles() {
        if (subtitleIndex < currentSpeech.speechParts.Count) { 
            // Check if first available part is playable
            if (currentSpeech.speechParts[subtitleIndex].GetTimestampInSeconds() <= subtitleTimer) {
                // Load next subtitle in variable
                currentSubtitle = currentSpeech.speechParts[subtitleIndex].text;
                if (PlayingSubtitles) {
                    subtitles.text = currentSubtitle;
                }
                // Point to next subtitle part
                subtitleIndex++;

                // If there are still speech parts, calculate timestamp in seconds
                if (currentSpeech.speechParts.Count > subtitleIndex) {
                    currentSpeech.speechParts[subtitleIndex].CalculateTimestampInSeconds();
                }
            }
        }
    }

    private void ResumeSubtitles() {
        subtitles.text = currentSubtitle;
    }

    private void PauseSubtitles() {
        subtitles.text = "";
    }

    /*
     * Getters and Setters
     */
    public int GetState() {
        return state;
    }
}
