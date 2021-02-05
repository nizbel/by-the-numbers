﻿using System;
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

    // File path constants
    private const string PATH_COMMON_JSON_SPEECH = "Json/Narrator/";
    private const string PATH_COMMON_AUDIO_SPEECH = "Sounds/Narrator/";
    private const string PATH_MOMENT_JSON_SPEECH = "Json/Narrator/Days/";
    private const string PATH_MOMENT_AUDIO_SPEECH = "Sounds/Narrator/Days/";

    private const float TIME_TO_WARN_AGAIN = 15.5f;

    public static NarratorController controller;

    public AudioSource narrator;
    public GameObject subtitles;

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
                subtitles.GetComponent<Text>().text = "";
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
        subtitles = GameObject.Find("Subtitles");

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

        currentSpeech = Instantiate(speech);
        // Prepare current speech timestamp in seconds
        currentSpeech.speechParts[0].CalculateTimestampInSeconds();
        Speak(currentSpeech.audio);

        // Prepare speech's unloading
        StartCoroutine(DestroySpeechInstance(currentSpeech));
    }

    IEnumerator DestroySpeechInstance(Speech speech) {
        yield return new WaitForSeconds(speech.audio.length);
        Destroy(speech);
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

            AudioClip clip = LoadCommonSpeech("Warning - Energy Level 1");
            Speak(clip);

            lastRangeWarning = Time.realtimeSinceStartup;
        }
    }

    public void WarnBarrier(bool positive) {
        if (state == QUIET && ShouldBarrierWarnAgain()) {
            state = WARNING;

            AudioClip clip;
            if (positive) {
                // TODO Choose from a pool of barrier warnings
                clip = LoadCommonSpeech("Positive barrier 1");
            } else {
                clip = LoadCommonSpeech("Negative barrier 1");
            }
            Speak(clip);

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

        subtitleTimer = 0;
    }

    public void StopSpeech() {
        narrator.Stop();
    }

    private Boolean ShouldRangeWarnAgain() {
        return lastRangeWarning == 0 || (Time.realtimeSinceStartup - lastRangeWarning) > TIME_TO_WARN_AGAIN;
    }

    private Boolean ShouldBarrierWarnAgain() {
        return lastRangeWarning == 0 || (Time.realtimeSinceStartup - lastRangeWarning) > TIME_TO_WARN_AGAIN;
    }

    private AudioClip LoadCommonSpeech(string jsonSpeech) {
        var jsonFile = Resources.Load<TextAsset>(PATH_COMMON_JSON_SPEECH + jsonSpeech);
        currentSpeech = JsonUtility.FromJson<Speech>(jsonFile.text);
        // Prepare current speech timestamp in seconds
        currentSpeech.speechParts[0].CalculateTimestampInSeconds();

        AudioClip clip = Resources.Load(PATH_COMMON_AUDIO_SPEECH + jsonSpeech) as AudioClip;
        return clip;
    }

    private AudioClip LoadMomentSpeech(string jsonSpeech) {
        var jsonFile = Resources.Load<TextAsset>(PATH_MOMENT_JSON_SPEECH + jsonSpeech);
        //Debug.Log(PATH_MOMENT_JSON_SPEECH + jsonSpeech);
        currentSpeech = JsonUtility.FromJson<Speech>(jsonFile.text);
        // Prepare current speech timestamp in seconds
        currentSpeech.speechParts[0].CalculateTimestampInSeconds();

        AudioClip clip = Resources.Load(PATH_MOMENT_AUDIO_SPEECH + jsonSpeech) as AudioClip;
        return clip;
    }

    private void PlaySubtitles() {
        if (currentSpeech.speechParts.Count > 0) {
            // Check if first available part is playable
            if (currentSpeech.speechParts[0].GetTimestampInSeconds() <= subtitleTimer) {
                // Load subtitle in variable
                currentSubtitle = currentSpeech.speechParts[0].text;
                if (PlayingSubtitles) {
                    subtitles.GetComponent<Text>().text = currentSubtitle;
                }
                currentSpeech.speechParts.RemoveAt(0);

                // If there are still speech parts, calculate timestamp in seconds
                if (currentSpeech.speechParts.Count > 0) {
                    currentSpeech.speechParts[0].CalculateTimestampInSeconds();
                }
            }
        }
    }

    private void ResumeSubtitles() {
        subtitles.GetComponent<Text>().text = currentSubtitle;
    }

    private void PauseSubtitles() {
        subtitles.GetComponent<Text>().text = "";
    }

    /*
     * Getters and Setters
     */
    public int GetState() {
        return state;
    }
}
