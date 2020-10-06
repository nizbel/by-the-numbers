using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFadeController : MonoBehaviour
{
    private const float FADE_IN_SPEED = 0.2f;
    private const float FADE_OUT_SPEED = 0.15f;

    [SerializeField]
    GameObject fadeInEffect = null;

    private bool fadingIn = true;

    public static ScreenFadeController controller;

    void Awake() {
        if (controller == null) {
            controller = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Disable ship input until the end of the fade in
        InputController.controller.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Control screen fading
        Color color = fadeInEffect.GetComponent<Image>().color;
        if (fadingIn) {
            color.a = Mathf.Lerp(color.a, color.a - FADE_IN_SPEED, Time.deltaTime);
            fadeInEffect.GetComponent<Image>().color = color;
            if (color.a <= 0) {
                EndFading();
                StageController.controller.SetState(StageController.COMMON_RANDOM_SPAWN_STATE);
            }
        } else {
            color.a = Mathf.Lerp(color.a, color.a + FADE_OUT_SPEED, Time.deltaTime);
            fadeInEffect.GetComponent<Image>().color = color;
            if (color.a >= 1) {
                EndFading();
                StageController.controller.SetState(StageController.ENDING_STATE);
            }
        }
    }

    void EndFading() {
        // Enable input controller
        InputController.controller.enabled = true;

        // Disable Fade Effect
        fadeInEffect.SetActive(false);

        // Disable this
        this.fadingIn = false;
        this.enabled = false;
    }
}
