using UnityEngine;
using UnityEngine.EventSystems;

// Input controller used when the game is paused
public class PausedInputController : MonoBehaviour {
		
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			StageController.controller.ResumeGame();
        }
	}
}
