using UnityEngine;
using System.Collections;
using System;
using System.IO;

[Serializable]
public class ScoreData {

	int skyHighScore;
	int infernoHighScore;
	int spaceHighScore;

	public ScoreData() {
		skyHighScore = 0;
		infernoHighScore = 0;
		spaceHighScore = 0;
	}

	public int getSkyHighScore() {
		return skyHighScore;
	}

	public void setSkyHighScore(int skyHighScore) {
		this.skyHighScore = skyHighScore;
	}

	public int getInfernoHighScore() {
		return infernoHighScore;
	}
	
	public void setInfernoHighScore(int infernoHighScore) {
		this.infernoHighScore = skyHighScore;
	}

	public int getSpaceHighScore() {
		return spaceHighScore;
	}
	
	public void setSpaceHighScore(int spaceHighScore) {
		this.spaceHighScore = spaceHighScore;
	}
}
