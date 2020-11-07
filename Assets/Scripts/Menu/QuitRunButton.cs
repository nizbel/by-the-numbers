using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuitRunButton : MonoBehaviour {
		
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void QuitRun() {
		StageController.controller.ResumeGame();

		// Disable narrator controller
		NarratorController.controller.GameOver();

		// Save data if story mode
		if (GameController.controller.GetState() == GameController.GAMEPLAY_STORY) {
			// TODO Save with unsuccesful times
			StageInfo stageInfo = GameController.GetGameInfo().GetStageInfoByDay(GameController.controller.GetCurrentDay());
			stageInfo.played = true;
			GameController.controller.Save();
		}

		GameController.controller.ChangeState(GameController.MAIN_MENU);
	}
}
