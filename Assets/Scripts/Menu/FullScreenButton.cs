using UnityEngine;
using UnityEngine.UI;

public class FullScreenButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Toggle>().isOn = Screen.fullScreen;

        // Apply listener
        GetComponent<Toggle>().onValueChanged.AddListener((value) => {
            GameController.SetFullScreen(value);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
