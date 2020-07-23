using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour {

	private bool playMusic = true;

	private bool playSFX = true;

	[SerializeField]
	public AudioMixer masterMixer;

	public static MusicController controller;
	
	void Awake() {
		if (controller == null) {
			controller = this;
			DontDestroyOnLoad(gameObject); 
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
