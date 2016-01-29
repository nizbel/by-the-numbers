using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {

	private bool playMusic = true;

	private bool playSFX = true;

	private GameObject musicObject;

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
		musicObject = GameObject.Find("Music");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnLevelWasLoaded(int scene) {
		musicObject = GameObject.Find("Music");
		if (!this.playMusic) {
			musicObject.audio.volume = 0;
		}
	}

	/*
	 * Getters and Setters
	 */
	public bool getPlayMusic() {
		return playMusic;
	}

	public void setPlayMusic(bool playMusic) {
		this.playMusic = playMusic;
		if (this.playMusic) {
			musicObject.audio.volume = 1;
		} else {
			musicObject.audio.volume = 0;
		}
	}

	public bool getPlaySFX() {
		return playSFX;
	}
	
	public void setPlaySFX(bool playSFX) {
		this.playSFX = playSFX;
	}
}
