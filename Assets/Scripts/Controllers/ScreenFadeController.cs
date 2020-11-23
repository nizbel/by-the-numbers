using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFadeController : MonoBehaviour
{
    private const float FADE_IN_SPEED = 0.45f;
    private const float FADE_OUT_SPEED = 0.35f;
    public const float GAME_OVER_FADE_OUT_SPEED = 0.75f;

    [SerializeField]
    GameObject darkScreen = null;

    private bool fadingIn = true;

    // Fading speeds
    float fadeInSpeed = FADE_IN_SPEED;
    float fadeOutSpeed = FADE_OUT_SPEED;

    // Show text for cutscene skipping
    private GameObject skipCutsceneText = null;

    // Stage ending animation
    StageEndingAnimation stageEndingAnimation;

    // UI Elements that shouldn't be visible during cutscenes
    FadingUIElement[] listFadingUIElements;

    public static ScreenFadeController controller;

    void Awake() {
        if (controller == null) {
            controller = this;

            // Load skip cutscene text
            skipCutsceneText = GameObject.Find("Skip Cutscene Text").gameObject;

            // Set stage ending animation
            stageEndingAnimation = GetComponent<StageEndingAnimation>();

            // Get UI elements
            listFadingUIElements = GameObject.FindObjectsOfType<FadingUIElement>();

        }
        else {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // If current moment is a cutscene, show skipping text
        skipCutsceneText.SetActive(GameController.GetGameInfo().StagePlayed(GameController.controller.GetCurrentDay()));
        this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Control screen fading
        Color color = darkScreen.GetComponent<SpriteRenderer>().color;
        if (fadingIn) {
            color.a = Mathf.Lerp(color.a, color.a - fadeInSpeed, Time.deltaTime);
            darkScreen.GetComponent<SpriteRenderer>().color = color;

            foreach (FadingUIElement element in listFadingUIElements) {
                element.SetAlpha(1-color.a);
            }

            if (color.a <= 0) {
                EndFading();
            }
        } else {
            color.a = Mathf.Lerp(color.a, color.a + fadeOutSpeed, Time.deltaTime);
            darkScreen.GetComponent<SpriteRenderer>().color = color;

            foreach (FadingUIElement element in listFadingUIElements) {
                element.SetAlpha(1-color.a);
            }

            if (color.a >= 1) {
                EndFading();

                // TODO Fix this idea, should be a separate method
                stageEndingAnimation.StartAnimation();
            }
        }
    }

    public void StartFadeIn() {
        // Enable fading
        this.enabled = true;

        // Hide skipping cutscene text
        skipCutsceneText.SetActive(false);
    }

    void EndFading() {
        if (fadingIn) {
            foreach (FadingUIElement element in listFadingUIElements) {
                element.SetAlpha(1);
            }
        } else {
            foreach (FadingUIElement element in listFadingUIElements) {
                element.SetAlpha(0);
            }
            // Show skip cutscene text if allowed
            skipCutsceneText.SetActive(StageController.controller.GetState() != StageController.GAME_OVER_STATE && GameController.GetGameInfo().StageDone(GameController.controller.GetCurrentDay()));
        }

        this.enabled = false;
    }

    public void StartFadeOut() {
        // Enable fading
        this.enabled = true;

        fadingIn = false;

        // Set background stars ahead
        stageEndingAnimation.enabled = true;
    }

    public void StartFadeOut(float fadeOutSpeed) {
        this.fadeOutSpeed = fadeOutSpeed;
        StartFadeOut();
    }
}
