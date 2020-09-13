using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [SerializeField]
    GameMenuController gameMenu;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PauseResume() {
        if (Time.timeScale == 0) {
            StageController.controller.ResumeGame();
        } else {
            StageController.controller.PauseGame();
        }
        gameMenu.gameObject.SetActive(!gameMenu.gameObject.activeSelf);
    }
}
