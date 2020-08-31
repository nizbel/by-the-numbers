using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour {

	private bool playMusic = true;

	private bool playSFX = true;

	[SerializeField]
	public AudioMixer masterMixer;

	public static MusicController controller;

	private const float LOWER_MUSIC_SFX_DURING_NARRATOR = 25;
	private float currentBGVolume = 0;
	private float currentSFXVolume = 0;

	//TODO get narrator in a cleaner way
	[SerializeField]
	public GameObject narrator;
	
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
			masterMixer.SetFloat("BackgroundVolume", 0);
		}
		else {
			masterMixer.SetFloat("BackgroundVolume", -80);
		}
	}

	public bool GetPlaySFX() {
		return playSFX;
	}
	
	public void SetPlaySFX(bool playSFX) {
		this.playSFX = playSFX;
		if (playSFX) {
			masterMixer.SetFloat("SFXVolume", 0);
		} else {
			masterMixer.SetFloat("SFXVolume", -80);
        }
	}
}
