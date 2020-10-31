using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFadeController : MonoBehaviour
{
    private const float FADE_IN_SPEED = 0.45f;
    private const float FADE_OUT_SPEED = 0.35f;
    public const float GAME_OVER_FADE_OUT_SPEED = 0.75f;

    [SerializeField]
    GameObject fadeInEffect = null;

    private bool fadingIn = true;

    // Fading speeds
    float fadeInSpeed = FADE_IN_SPEED;
    float fadeOutSpeed = FADE_OUT_SPEED;

    public static ScreenFadeController controller;

    void Awake() {
        if (controller == null) {
            controller = this;
            this.enabled = false;
        }
        else {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Disable ship input until the end of the fade in
        // TODO fix this
        //InputController.controller.enabled = false;

        // TODO Remove this test
        // Insert day number in screen fade
        fadeInEffect.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Day " + GameController.controller.GetCurrentDay();
        fadeInEffect.transform.GetChild(0).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        // Control screen fading
        Color color = fadeInEffect.GetComponent<Image>().color;
        if (fadingIn) {
            color.a = Mathf.Lerp(color.a, color.a - fadeInSpeed, Time.deltaTime);
            fadeInEffect.GetComponent<Image>().color = color;

            // TODO Remove code for day signaling
            Color textColor = fadeInEffect.transform.GetChild(0).gameObject.GetComponent<Text>().color;
            textColor.a = color.a;
            fadeInEffect.transform.GetChild(0).gameObject.GetComponent<Text>().color = textColor;
            if (color.a <= 0) {
                EndFading();
            }
        } else {
            color.a = Mathf.Lerp(color.a, color.a + fadeOutSpeed, Time.deltaTime);
            fadeInEffect.GetComponent<Image>().color = color;
            if (color.a >= 1) {
                EndFading();
            }
        }
    }

    public void StartFadeIn() {
        // Enable fading
        this.enabled = true;
    }

    void EndFading() {
        // Enable input controller
        // TODO FIX this
        //InputController.controller.enabled = true;

        if (fadingIn) {
            // Disable Fade Effect
            fadeInEffect.SetActive(false);

            // Disable fading
            //this.fadingIn = false;
        }

        this.enabled = false;

        // TODO Remove the code for day signaling
        if (fadeInEffect.transform.childCount > 0) {
            Destroy(fadeInEffect.transform.GetChild(0).gameObject);
        }
    }

    public void StartFadeOut() {
        // Enable fading
        this.enabled = true;

        fadingIn = false;

        // Enable input controller
        //InputController.controller.enabled = false;

        // Disable Fade Effect
        fadeInEffect.SetActive(true);
    }

    public void StartFadeOut(float fadeOutSpeed) {
        this.fadeOutSpeed = fadeOutSpeed;
        StartFadeOut();
    }
}
