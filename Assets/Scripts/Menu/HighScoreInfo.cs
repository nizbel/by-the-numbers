using UnityEngine;
using UnityEngine.UI;

public class HighScoreInfo : MonoBehaviour
{
    private Text dayText;
    private Text highScoreText;
    private Text winRateText;

    // Start is called before the first frame update
    void Awake()
    {
        dayText = transform.Find("Day").GetComponent<Text>();
        highScoreText = transform.Find("High Score").GetComponent<Text>();
        winRateText = transform.Find("Win Rate").GetComponent<Text>();
    }


    public void SetScoreInfo(int day, int highScore, float winRate) {
        dayText.text = "Day " + day;
        highScoreText.text = highScore.ToString();
        winRateText.text = winRate.ToString("n1") + "%";
    }

    public void SetAsHeader() {
        dayText.text = "Day";
        highScoreText.text = "Best";
        winRateText.text = "Win %";
    }
}
