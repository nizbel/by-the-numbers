﻿using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameController : MonoBehaviour {

	/*
	 * LEVEL CONSTANTS
	 */
	public const int MAIN_MENU = 0;
	public const int GAMEPLAY = 1;

	ScoreData scoreData;

	public bool gameStarted = false;

	public bool gamePaused = false;

	public GameObject player;

	public static GameController controller;

	/*
	 * Maps the current state of the game
	 * 0 = Main Menu
	 * 1 = Intro
	 * 2 = Farm run
	 * 3 = Sky
	 * 4 = Inferno
	 * 5 = Space
	 * 6 = Win state
	 * 7 = Loss state
	 */
	public int state = 0;
	
	void Awake() {
		if (controller == null) {
			controller = this;
			switch (Application.platform) {
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.WindowsPlayer:
				Screen.SetResolution(1136, 640, false);
				break;
			case RuntimePlatform.Android:
				Screen.SetResolution(1136, 640, false);
				break;
			}
			DontDestroyOnLoad(gameObject);
			scoreData = new ScoreData();
			Load();
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

	public int getState() {
		return state;
	}

	public void changeState(int newState) {
		switch(newState) {
		case MAIN_MENU:
			gameStarted = false;
			Application.LoadLevel(MAIN_MENU);
			break;
		case GAMEPLAY:
			gameStarted = true;
			Application.LoadLevel(GAMEPLAY);
			break;
		}
		state = newState;
	}

	public void Save() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "ScoreData.lhd");
		
		bf.Serialize(file, scoreData);
		
		file.Close();
	}
	
	public void Load() {
		if (File.Exists(Application.persistentDataPath + "ScoreData.lhd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "ScoreData.lhd", FileMode.Open);
			
			scoreData = (ScoreData) bf.Deserialize(file);
			
			file.Close();
		}
	}

	public ScoreData getScoreData() {
		return scoreData;
	}
}
