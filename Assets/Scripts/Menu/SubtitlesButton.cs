using UnityEngine;
using UnityEngine.UI;

public class SubtitlesButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Toggle>().isOn = NarratorController.controller.PlayingSubtitles;

        // Apply listener for NarratorController
        GetComponent<Toggle>().onValueChanged.AddListener((value) => {
            NarratorController.controller.PlayingSubtitles = value;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
