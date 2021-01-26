using UnityEngine;
using UnityEngine.UI;

public class HighScoreLoader : MonoBehaviour
{
    private const float DAY_INFO_POSITION_X = -400;
    private const float CONSTELLATION_SEEN_INFO_POSITION_X = 300;

    public GameObject highScoreInfoPrefab = null;
    public GameObject constellationSeenInfoPrefab = null;

    public Constellation[] constellations = new Constellation[ConstellationController.AVAILABLE_CONSTELLATIONS.Length];

    // Start is called before the first frame update
    void Start() {
        GameInfo gameInfo = GameController.GetGameInfo();
        LoadHighScore(gameInfo);

        // TODO Load constellation info
        for (int i = 0; i < gameInfo.constellationInfo.Length; i++) {
            ConstellationInfo currentConstellationInfo = gameInfo.constellationInfo[i];
            if (currentConstellationInfo == null) {
                constellations[i] = null;
            }
        }

        // Fill constellation info
        transform.Find("Constellations Seen").GetComponent<Text>().text = "Constellations Seen";

        Vector3 position = new Vector3(CONSTELLATION_SEEN_INFO_POSITION_X, 230, 0);

        // Load played days
        for (int i = 0; i < gameInfo.constellationInfo.Length; i++) {
            ConstellationInfo currentConstellationInfo = gameInfo.constellationInfo[i];

            // Instantiate one UI component for each constellation
            GameObject newConstellationSeenInfo = GameObject.Instantiate(constellationSeenInfoPrefab, transform);
            newConstellationSeenInfo.GetComponent<RectTransform>().anchoredPosition = position;
            newConstellationSeenInfo.GetComponent<RectTransform>().localScale = Vector3.one;

            // Set day and score data
            if (currentConstellationInfo != null) {
                // Get constellation data
                Constellation constellation = constellations[i];
                newConstellationSeenInfo.GetComponent<ConstellationSeenInfo>().SetConstellationName(constellation.name);
                newConstellationSeenInfo.GetComponent<ConstellationSeenInfo>().SetTimesSeen(currentConstellationInfo.timesSeen);
            } else {
                newConstellationSeenInfo.GetComponent<ConstellationSeenInfo>().SetConstellationName("???");
                newConstellationSeenInfo.GetComponent<ConstellationSeenInfo>().SetTimesSeen(0);
            }

            position += Vector3.down * 30;
        }

        // Hide text at start
        System.Array.ForEach(gameObject.GetComponentsInChildren<Text>(), x => x.enabled = false);
    }

    private void LoadHighScore(GameInfo gameInfo) {
        // Headers
        GameObject headerInfo = GameObject.Instantiate(highScoreInfoPrefab, transform);
        headerInfo.GetComponent<RectTransform>().anchoredPosition = new Vector3(DAY_INFO_POSITION_X, 300, 0);
        headerInfo.GetComponent<RectTransform>().localScale = Vector3.one;

        // Set day and score data
        headerInfo.GetComponent<HighScoreInfo>().SetAsHeader();

        Vector3 scorePosition = new Vector3(DAY_INFO_POSITION_X, 270, 0);

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
    }
}
