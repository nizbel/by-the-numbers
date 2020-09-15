using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NarratorController : MonoBehaviour
{
    public const int QUIET = 0;
    public const int IMPORTANT = 1;
    public const int WARNING = 2;

    public static NarratorController controller;

    //TODO clean this
    [SerializeField]
    public GameObject narrator;

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

    private const string PATH_JSON_SPEECH = "Json/Narrator/";
    private const string PATH_AUDIO_SPEECH = "Sounds/Narrator/";

    void Awake() {
        if (controller == null) {
            controller = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gameRunning) {
            if (!narrator.GetComponent<AudioSource>().isPlaying && state != QUIET && Time.timeScale > 0) {
                state = QUIET;
                MusicController.controller.IncreaseVolumeAfterNarrator();

                // Clear subtitle variables
                narrator.GetComponentInChildren<TextMesh>().text = "";
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

        gameRunning = true;

        state = IMPORTANT;

        AudioClip clip = LoadSpeech(PATH_JSON_SPEECH + "Olivia-start");
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

            AudioClip clip = LoadSpeech(PATH_JSON_SPEECH + "Olivia-warn-range");
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

    private AudioClip LoadSpeech(string jsonSpeech) {
        var jsonFile = Resources.Load<TextAsset>(jsonSpeech);
        currentSpeech = JsonUtility.FromJson<Speech>(jsonFile.text);
        AudioClip clip = Resources.Load(PATH_AUDIO_SPEECH + currentSpeech.speechAudio) as AudioClip;
        return clip;
    }

    private void PlaySubtitles() {
        if (currentSpeech.speech.Count > 0) {
            // Check if first available part is playable
            if (ConvertTimestampToSeconds(currentSpeech.speech[0].timestamp) <= subtitleTimer) {
                // Load subtitle in variable
                currentSubtitle = currentSpeech.speech[0].text;
                if (PlayingSubtitles) {
                    narrator.GetComponentInChildren<TextMesh>().text = currentSubtitle;
                }
                currentSpeech.speech.RemoveAt(0);
            }
        }
    }

    private void ResumeSubtitles() {
        narrator.GetComponentInChildren<TextMesh>().text = currentSubtitle;
    }

    private void PauseSubtitles() {
        narrator.GetComponentInChildren<TextMesh>().text = "";
    }


    private int ConvertTimestampToSeconds(string timestamp) {
        string[] timestampParts = timestamp.Split(':');
        return int.Parse(timestampParts[0]) * 60 + int.Parse(timestampParts[1]);
    }
}
