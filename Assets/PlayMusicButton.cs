using UnityEngine;
using UnityEngine.UI;

public class PlayMusicButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Toggle>().isOn = MusicController.controller.GetPlayMusic();

        // Apply listener for MusicController
        GetComponent<Toggle>().onValueChanged.AddListener((value) => {
            MusicController.controller.SetPlayMusic(value);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
