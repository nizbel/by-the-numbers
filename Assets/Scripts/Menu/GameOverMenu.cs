using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GoToMainMenu()
    {
        GameController.controller.ChangeState(GameController.MAIN_MENU);
    }

    public void TryAgain() {
        if (GameController.controller.GetState() == GameController.GAME_OVER_STORY) {
            GameController.controller.SetCurrentDay(CurrentDayController.GetInitialDay());
            GameController.controller.ChangeState(GameController.GAMEPLAY_STORY);
        } else {
            GameController.controller.ChangeState(GameController.GAMEPLAY_INFINITE);
        }
    }
}
