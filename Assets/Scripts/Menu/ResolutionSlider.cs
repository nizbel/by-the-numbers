using UnityEngine;
using UnityEngine.UI;

public class ResolutionSlider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        switch (Screen.currentResolution.width) {
            case GameController.FHD_WINDOWS_RES_X:
                GetComponent<Slider>().value = GameController.WINDOWS_FHD_RES;
                break;

            case GameController.HD_WINDOWS_RES_X:
                GetComponent<Slider>().value = GameController.WINDOWS_HD_RES;
                break;
        }

        // Apply listener for NarratorController
        GetComponent<Slider>().onValueChanged.AddListener((value) => {
            GameController.SetResolution(Mathf.RoundToInt(value));
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
