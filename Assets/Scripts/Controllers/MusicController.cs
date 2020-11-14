using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour {

	// Volume constants
	private const float VOLUME_OFF = -80;
	private const float INITIAL_VOLUME = 0;
	private const float LOWER_MUSIC_SFX_DURING_NARRATOR = 25;

	// Mixer name constants
	private const string MUSIC_MIXER = "BackgroundVolume";
	private const string SFX_MIXER = "SFXVolume";

	private bool playMusic = true;

	private bool playSFX = true;

	[SerializeField]
	public AudioMixer masterMixer;

	public static MusicController controller;

	private float currentBGVolume = INITIAL_VOLUME;
	private float currentSFXVolume = INITIAL_VOLUME;
	
	void Awake() {
		if (controller == null) {
			controller = this;
			DontDestroyOnLoad(gameObject);

			// Get volumes
			masterMixer.GetFloat(MUSIC_MIXER, out currentBGVolume);
			masterMixer.GetFloat(SFX_MIXER, out currentSFXVolume);

			// Get values from player prefs
			playMusic = PlayerPrefsUtil.GetBoolPref(PlayerPrefsUtil.PLAY_MUSIC_PREF);
			playSFX = PlayerPrefsUtil.GetBoolPref(PlayerPrefsUtil.PLAY_SFX_PREF);
		}
		else {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		// Set values from player prefs into mixer
		if (!playMusic) {
			currentBGVolume = VOLUME_OFF;
			masterMixer.SetFloat(MUSIC_MIXER, currentBGVolume);
		}
		if (!playSFX) {
			currentSFXVolume = VOLUME_OFF;
			masterMixer.SetFloat(SFX_MIXER, currentSFXVolume);
		}
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void DecreaseVolumeForNarrator() {
		masterMixer.SetFloat(MUSIC_MIXER, currentBGVolume - LOWER_MUSIC_SFX_DURING_NARRATOR);
		masterMixer.SetFloat(SFX_MIXER, currentSFXVolume - LOWER_MUSIC_SFX_DURING_NARRATOR);
	}

	public void IncreaseVolumeAfterNarrator() {
		masterMixer.SetFloat(MUSIC_MIXER, currentBGVolume);
		masterMixer.SetFloat(SFX_MIXER, currentSFXVolume);
	}

	/*
	 * Getters and Setters
	 */
	public bool GetPlayMusic() {
		return playMusic;
	}

	public void SetPlayMusic(bool playMusic) {
		this.playMusic = playMusic;
		if (playMusic) {
			currentBGVolume = INITIAL_VOLUME;
		}
		else {
			currentBGVolume = VOLUME_OFF;
		}
		masterMixer.SetFloat(MUSIC_MIXER, currentBGVolume);

		// Update player prefs
		PlayerPrefsUtil.SetBoolPref(PlayerPrefsUtil.PLAY_MUSIC_PREF, playMusic);
	}

	

	public bool GetPlaySFX() {
		return playSFX;
	}
	
	public void SetPlaySFX(bool playSFX) {
		this.playSFX = playSFX;
		if (playSFX) {
			currentSFXVolume = INITIAL_VOLUME;
		} else {
			currentSFXVolume = VOLUME_OFF;
		}
		masterMixer.SetFloat(SFX_MIXER, currentSFXVolume);

		// Update player prefs
		PlayerPrefsUtil.SetBoolPref(PlayerPrefsUtil.PLAY_SFX_PREF, playSFX);
	}

	public void SetPitch(float pitch) {
		masterMixer.SetFloat("BackgroundPitch", pitch);
		masterMixer.SetFloat("SFXPitch", pitch);
	}

	public void SetMusicVolumeInGame(float volume) {
		AudioSource musicScript = GameObject.Find("Music").GetComponent<AudioSource>();
		musicScript.volume = volume;
    }
}
