using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [SerializeField]
    Image pauseDarken;

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
        pauseDarken.gameObject.SetActive(!pauseDarken.gameObject.activeSelf);
    }
}
