using UnityEngine;
using UnityEngine.UI;

public class HighScoreInfo : MonoBehaviour
{
    private Text dayText;
    private Text highScoreText;

    // Start is called before the first frame update
    void Awake()
    {
        dayText = transform.Find("Day").GetComponent<Text>();
        highScoreText = transform.Find("High Score").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScoreInfo(int day, int highScore) {
        dayText.text = "Day " + day;
        highScoreText.text = highScore.ToString();
    }
}
