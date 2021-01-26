using UnityEngine;
using UnityEngine.UI;

public class ConstellationSeenInfo : MonoBehaviour
{
    private Text constellationName;
    private Text timesSeen;

    // Start is called before the first frame update
    void Awake()
    {
        constellationName = transform.Find("Name").GetComponent<Text>();
        timesSeen = transform.Find("Times Seen").GetComponent<Text>();
    }

    public void SetConstellationName(string constellationName) {
        this.constellationName.text = constellationName;
    }

    public void SetTimesSeen(int timesSeen) {
        this.timesSeen.text = timesSeen.ToString();
    }
}
