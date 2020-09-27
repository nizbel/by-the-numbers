using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour {

	private const float VOLUME_OFF = -80;
	private const float INITIAL_VOLUME = 0;

	private bool playMusic = true;

	private bool playSFX = true;

	[SerializeField]
	public AudioMixer masterMixer;

	public static MusicController controller;

	private const float LOWER_MUSIC_SFX_DURING_NARRATOR = 25;
	private float currentBGVolume = INITIAL_VOLUME;
	private float currentSFXVolume = INITIAL_VOLUME;
	
	void Awake() {
		if (controller == null) {
			controller = this;
			DontDestroyOnLoad(gameObject);

			// Get volumes
			masterMixer.GetFloat("BackgroundVolume", out currentBGVolume);
			masterMixer.GetFloat("SFXVolume", out currentSFXVolume);
		}
		else {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void DecreaseVolumeForNarrator() {
		masterMixer.SetFloat("BackgroundVolume", currentBGVolume - LOWER_MUSIC_SFX_DURING_NARRATOR);
		masterMixer.SetFloat("SFXVolume", currentSFXVolume - LOWER_MUSIC_SFX_DURING_NARRATOR);
	}

	public void IncreaseVolumeAfterNarrator() {

		masterMixer.SetFloat("BackgroundVolume", currentBGVolume);
		masterMixer.SetFloat("SFXVolume", currentSFXVolume);
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
		masterMixer.SetFloat("BackgroundVolume", currentBGVolume);
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
		masterMixer.SetFloat("SFXVolume", currentSFXVolume);
	}
}
