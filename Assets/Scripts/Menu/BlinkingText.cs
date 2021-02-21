using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingText : MonoBehaviour
{
    [SerializeField]
    float minAlpha = 0;
    [SerializeField]
    float maxAlpha = 1;

    float currentAlpha;

    // A blink is going to the other alpha extreme and returning
    [SerializeField]
    float blinksPerSecond = 1;

    bool startVisible = false;
    int direction = 1;

    Text text = null;

    void OnEnable() {
        if (text == null) {
            text = GetComponent<Text>();
        }
        StartAlpha();
    }

    void Update()
    {
        currentAlpha += (maxAlpha - minAlpha) * Time.deltaTime * direction * blinksPerSecond * 2;

        if (direction == 1 && currentAlpha >= maxAlpha) {
            direction = -1;
            currentAlpha = maxAlpha;
        } else if (direction == -1 && currentAlpha <= minAlpha) {
            direction = 1;
            currentAlpha = minAlpha;
        }

        ChangeAlpha(currentAlpha);
    }

    void ChangeAlpha(float alpha) {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }

    void StartAlpha() {
        if (startVisible) {
            currentAlpha = maxAlpha;
            direction = -1;
        }
        else {
            currentAlpha = minAlpha;
        }
        ChangeAlpha(currentAlpha);
    }
}
