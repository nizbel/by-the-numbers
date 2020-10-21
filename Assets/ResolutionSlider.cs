using UnityEngine;
using UnityEngine.UI;

public class ResolutionSlider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        switch (Screen.currentResolution.width) {
            case 1920:
                GetComponent<Slider>().value = 2;
                break;

            case 1366:
                GetComponent<Slider>().value = 1;
                break;
        }

        // Apply listener for NarratorController
        GetComponent<Slider>().onValueChanged.AddListener((value) => {
            switch (value) {
                case 1:
                    Screen.SetResolution(1366, 768, Screen.fullScreen);
                    break;

                case 2:
                    Screen.SetResolution(GameController.WINDOWS_RES_X, GameController.WINDOWS_RES_Y, Screen.fullScreen);
                    break;
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
