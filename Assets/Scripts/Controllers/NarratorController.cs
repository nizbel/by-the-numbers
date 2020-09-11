using System;
using System.Collections;
using System.Collections.Generic;
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

    private int state = QUIET;

    private float lastRangeWarning = 0;

    private Speech currentSpeech = null;

    private bool playingSubtitles = false;
    private float subtitleTimer = 0;

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
        state = IMPORTANT;

        //Speak();

        LoadSpeech();


        var clip = Resources.Load("Sounds/Narrator/" + currentSpeech.speechAudio) as AudioClip;
        Speak(clip);
    }

    // Update is called once per frame
    void Update()
    {
        if (!narrator.GetComponent<AudioSource>().isPlaying && state != QUIET) {
            state = QUIET;
            MusicController.controller.IncreaseVolumeAfterNarrator();
            narrator.GetComponentInChildren<TextMesh>().text = "";
        }

        if (playingSubtitles) {
            PlaySubtitles();
            subtitleTimer += Time.deltaTime;
        }
    }

    public void WarnRange() {
        if (state == QUIET && ShouldWarnAgain()) {
            state = WARNING;
            var clip = Resources.Load("Sounds/Narrator/Olivia-warn-range") as AudioClip;
            Speak(clip);

            lastRangeWarning = Time.realtimeSinceStartup;
        }
    }

    private void Speak() {
        MusicController.controller.DecreaseVolumeForNarrator();
        narrator.GetComponent<AudioSource>().Play();

        playingSubtitles = true;
    }

    private void Speak(AudioClip clip) {
        MusicController.controller.DecreaseVolumeForNarrator();
        narrator.GetComponent<AudioSource>().clip = clip;
        narrator.GetComponent<AudioSource>().Play();

        playingSubtitles = true;
        subtitleTimer = 0;
    }

    private Boolean ShouldWarnAgain() {
        return lastRangeWarning == 0 || (Time.realtimeSinceStartup - lastRangeWarning) > 10;
    }

    private void LoadSpeech() {
        var jsonFile = Resources.Load<TextAsset>("Json/Narrator/Olivia-start");
        currentSpeech = JsonUtility.FromJson<Speech>(jsonFile.text);
    }

    private void PlaySubtitles() {
        if (currentSpeech.speech.Count > 0) {
            // Check if first available part is playable
            if (ConvertTimestampToSeconds(currentSpeech.speech[0].timestamp) <= subtitleTimer) {
                narrator.GetComponentInChildren<TextMesh>().text = currentSpeech.speech[0].text;
                currentSpeech.speech.RemoveAt(0);
            }
        }
    }

    private int ConvertTimestampToSeconds(string timestamp) {
        string[] timestampParts = timestamp.Split(':');
        return int.Parse(timestampParts[0]) * 60 + int.Parse(timestampParts[1]);
    }
}
