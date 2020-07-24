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
        Speak();
    }

    // Update is called once per frame
    void Update()
    {
        if (!narrator.GetComponent<AudioSource>().isPlaying && state != QUIET) {
            state = QUIET;
            MusicController.controller.IncreaseVolumeAfterNarrator();
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
    }

    private void Speak(AudioClip clip) {
        MusicController.controller.DecreaseVolumeForNarrator();
        narrator.GetComponent<AudioSource>().clip = clip;
        narrator.GetComponent<AudioSource>().Play();
    }

    private Boolean ShouldWarnAgain() {
        return lastRangeWarning == 0 || (Time.realtimeSinceStartup - lastRangeWarning) > 10;
    }
}
