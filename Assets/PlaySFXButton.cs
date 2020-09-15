using UnityEngine;
using UnityEngine.UI;

public class PlaySFXButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Toggle>().isOn = MusicController.controller.GetPlaySFX();

        // Apply listener for MusicController
        GetComponent<Toggle>().onValueChanged.AddListener((value) => {
            MusicController.controller.SetPlaySFX(value);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
