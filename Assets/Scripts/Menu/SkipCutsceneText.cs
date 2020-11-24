using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipCutsceneText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // If current moment is a cutscene, show skipping text
        gameObject.SetActive(GameController.GetGameInfo().StagePlayed(GameController.controller.GetCurrentDay()));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
