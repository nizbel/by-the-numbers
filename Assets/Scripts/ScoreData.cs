using UnityEngine;
using System.Collections;
using System;
using System.IO;

[Serializable]
public class ScoreData {

	int highScore;

	public ScoreData() {
		highScore = 0;
	}

	public int getHighScore() {
		return highScore;
	}

	public void setHighScore(int highScore) {
		this.highScore = highScore;
	}

}
