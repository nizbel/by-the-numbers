using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BlinkingSprite : MonoBehaviour
{
    private const float DEFAULT_DURATION = 2.2f;

    private float duration = 0;

    private float createdAt;

    [SerializeField]
    private float blinkFrequency = 0.3f;

    private float lastBlink;

    [SerializeField]
    private Light2D spriteLight = null;

    // Start is called before the first frame update
    void Start()
    {
        if (duration == 0) {
            duration = DEFAULT_DURATION;
        }
        createdAt = Time.time;
        lastBlink = createdAt;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastBlink > blinkFrequency) {
            spriteLight.intensity = (spriteLight.intensity + 1)%2;
            lastBlink = Time.time;
        } else if (duration > 0 && Time.time - createdAt > duration) {
            Destroy(this.gameObject);
        }
    }
}
