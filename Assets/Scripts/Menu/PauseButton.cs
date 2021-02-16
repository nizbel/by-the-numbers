using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    public void PauseResume() {
        if (StageController.controller.GetGamePaused()) {
            StageController.controller.ResumeGame();
        } else {
            StageController.controller.PauseGame();
        }
    }
}
