using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour {

	private bool playMusic = true;

	private bool playSFX = true;

	private GameObject musicObject;

	public static MusicController controller;
	
	void Awake() {
		if (controller == null) {
			controller = this;
			DontDestroyOnLoad(gameObject); 
			SceneManager.sceneLoaded += OnLevelFinishedLoading;
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

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
		musicObject = GameObject.Find("Music");
		if (!this.playMusic) {
			musicObject.GetComponent<AudioSource>().volume = 0;
		}
	}

	/*
	 * Getters and Setters
	 */
	public bool GetPlayMusic() {
		return playMusic;
	}

	public void SetPlayMusic(bool playMusic) {
		this.playMusic = playMusic;
		if (this.playMusic) {
			musicObject.GetComponent<AudioSource>().volume = 1;
		} else {
			musicObject.GetComponent<AudioSource>().volume = 0;
		}
	}

	public bool GetPlaySFX() {
		return playSFX;
	}
	
	public void SetPlaySFX(bool playSFX) {
		this.playSFX = playSFX;
	}
}
