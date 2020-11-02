using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreLoader : MonoBehaviour
{
    public GameObject highScoreInfoPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        GameInfo gameInfo = GameController.GetGameInfo();

        Vector3 scorePosition = new Vector3(50, 300, 0);

        // Keep track of how many days player finished
        int amountOfDaysFinished = 0;

        // Load played days
        foreach (StageInfo stageInfo in gameInfo.ListStageInfoByDay()) {
            // Instantiate one UI component for each stage info
            GameObject newHighScoreInfo = GameObject.Instantiate(highScoreInfoPrefab, transform);
            newHighScoreInfo.GetComponent<RectTransform>().anchoredPosition = scorePosition;
            newHighScoreInfo.GetComponent<RectTransform>().localScale = Vector3.one;

            // Set day and score data
            newHighScoreInfo.GetComponent<HighScoreInfo>().SetScoreInfo(stageInfo.day, stageInfo.highScore);

            scorePosition += Vector3.down * 40;

            if (stageInfo.highScore > 0) {
                amountOfDaysFinished++;
            }
        }

        // Fill story mode progress
        int amountOfDaysAvailable = CurrentDayController.GetDaysAvailable().Count;
        float storyProgress = 100f * amountOfDaysFinished / amountOfDaysAvailable;
        transform.Find("Story Progress").GetComponent<Text>().text = "Story Mode: " + storyProgress.ToString("n0") + "%";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
