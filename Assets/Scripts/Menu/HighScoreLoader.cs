using UnityEngine;
using UnityEngine.UI;

public class HighScoreLoader : MonoBehaviour
{
    public GameObject highScoreInfoPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        GameInfo gameInfo = GameController.GetGameInfo();

        // Headers
        GameObject headerInfo = GameObject.Instantiate(highScoreInfoPrefab, transform);
        headerInfo.GetComponent<RectTransform>().anchoredPosition = new Vector3(50, 300, 0);
        headerInfo.GetComponent<RectTransform>().localScale = Vector3.one;

        // Set day and score data
        headerInfo.GetComponent<HighScoreInfo>().SetAsHeader();

        Vector3 scorePosition = new Vector3(50, 270, 0);

        // Keep track of how many days player finished
        int amountOfDaysFinished = 0;

        // Load played days
        foreach (StageInfo stageInfo in gameInfo.ListStageInfoByDay()) {
            // Instantiate one UI component for each stage info
            GameObject newHighScoreInfo = GameObject.Instantiate(highScoreInfoPrefab, transform);
            newHighScoreInfo.GetComponent<RectTransform>().anchoredPosition = scorePosition;
            newHighScoreInfo.GetComponent<RectTransform>().localScale = Vector3.one;

            // Set day and score data
            newHighScoreInfo.GetComponent<HighScoreInfo>().SetScoreInfo(stageInfo.day, stageInfo.highScore, stageInfo.WinRate());

            scorePosition += Vector3.down * 30;

            if (stageInfo.IsDone()) {
                amountOfDaysFinished++;
            }
        }

        // Fill story mode progress
        int amountOfDaysAvailable = CurrentDayController.GetDaysAvailable().Count;
        float storyProgress = 100f * amountOfDaysFinished / amountOfDaysAvailable;
        transform.Find("Story Progress").GetComponent<Text>().text = "Story Mode: " + storyProgress.ToString("n0") + "%";

        // Fill high score for infinite
        transform.Find("Infinite High Score").GetComponent<Text>().text = "Infinite Mode: " + gameInfo.infiniteHighScore;

        // Hide text at start
        System.Array.ForEach(gameObject.GetComponentsInChildren<Text>(), x => x.enabled = false);
    }
}
