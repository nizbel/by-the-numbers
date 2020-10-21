using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NarratorController : MonoBehaviour {
    public const int QUIET = 0;
    public const int IMPORTANT = 1;
    public const int WARNING = 2;

    public static NarratorController controller;

    public GameObject narrator;
    public GameObject subtitles;

    private bool gameRunning = false;

    private int state = QUIET;

    private float lastRangeWarning = 0;

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
        }
    }
    private string currentSubtitle = "";
    private float subtitleTimer = 0;

    private const string PATH_COMMON_JSON_SPEECH = "Json/Narrator/";
    private const string PATH_COMMON_AUDIO_SPEECH = "Sounds/Narrator/";
    private const string PATH_EVENT_JSON_SPEECH = "Json/Narrator/Days/";
    private const string PATH_EVENT_AUDIO_SPEECH = "Sounds/Narrator/Days/";

    void Awake() {
        if (controller == null) {
            controller = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (gameRunning) {
            if (!narrator.GetComponent<AudioSource>().isPlaying && state != QUIET && Time.timeScale > 0) {
                state = QUIET;
                MusicController.controller.IncreaseVolumeAfterNarrator();

                // Clear subtitle variables
                subtitles.GetComponent<Text>().text = "";
                currentSubtitle = "";
            }
            else if (narrator.GetComponent<AudioSource>().isPlaying) {
                PlaySubtitles();

                subtitleTimer += Time.deltaTime;
            }
        }
    }

    public void StartGame() {
        // Find narrator
        narrator = GameObject.Find("Narrator");
        // Find subtitles
        subtitles = GameObject.Find("Subtitles");

        gameRunning = true;
    }

    public void StartEventSpeech(string jsonSpeech) {
        if (state == IMPORTANT) {
            Debug.LogError("Important speeches can't be stopped");
        }
        state = IMPORTANT;

        AudioClip clip = LoadEventSpeech(jsonSpeech);
        Speak(clip);
    }

    public void GameOver() {
        narrator = null;

        gameRunning = false;

        if (state != QUIET) {
            MusicController.controller.IncreaseVolumeAfterNarrator();
        }
    }

    public void WarnRange() {
        if (state == QUIET && ShouldWarnAgain()) {
            state = WARNING;

            AudioClip clip = LoadCommonSpeech("Warning - Energy Level 1");
            Speak(clip);

            lastRangeWarning = Time.realtimeSinceStartup;
        }
    }

    //private void Speak() {
    //    MusicController.controller.DecreaseVolumeForNarrator();
    //    narrator.GetComponent<AudioSource>().Play();
    //}

    private void Speak(AudioClip clip) {
        MusicController.controller.DecreaseVolumeForNarrator();
        narrator.GetComponent<AudioSource>().clip = clip;
        narrator.GetComponent<AudioSource>().Play();

        subtitleTimer = 0;
    }

    private Boolean ShouldWarnAgain() {
        return lastRangeWarning == 0 || (Time.realtimeSinceStartup - lastRangeWarning) > 10;
    }

    private AudioClip LoadCommonSpeech(string jsonSpeech) {
        var jsonFile = Resources.Load<TextAsset>(PATH_COMMON_JSON_SPEECH + jsonSpeech);
        currentSpeech = JsonUtility.FromJson<Speech>(jsonFile.text);
        // Prepare current speech timestamp in seconds
        currentSpeech.speech[0].CalculateTimestampInSeconds();

        AudioClip clip = Resources.Load(PATH_COMMON_AUDIO_SPEECH + currentSpeech.speechAudio) as AudioClip;
        return clip;
    }

    private AudioClip LoadEventSpeech(string jsonSpeech) {
        var jsonFile = Resources.Load<TextAsset>(PATH_EVENT_JSON_SPEECH + jsonSpeech);
        //Debug.Log(PATH_EVENT_JSON_SPEECH + jsonSpeech);
        currentSpeech = JsonUtility.FromJson<Speech>(jsonFile.text);
        // Prepare current speech timestamp in seconds
        currentSpeech.speech[0].CalculateTimestampInSeconds();

        AudioClip clip = Resources.Load(PATH_EVENT_AUDIO_SPEECH + currentSpeech.speechAudio) as AudioClip;
        return clip;
    }

    private void PlaySubtitles() {
        if (currentSpeech.speech.Count > 0) {
            // Check if first available part is playable
            if (currentSpeech.speech[0].GetTimestampInSeconds() <= subtitleTimer) {
                // Load subtitle in variable
                currentSubtitle = currentSpeech.speech[0].text;
                if (PlayingSubtitles) {
                    subtitles.GetComponent<Text>().text = currentSubtitle;
                }
                currentSpeech.speech.RemoveAt(0);

                // If there are still speech parts, calculate timestamp in seconds
                if (currentSpeech.speech.Count > 0) {
                    currentSpeech.speech[0].CalculateTimestampInSeconds();
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
