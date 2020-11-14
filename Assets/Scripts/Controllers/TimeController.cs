using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController controller;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTimeScale(float timeScale, bool changePitch=true) {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        if (changePitch) {
            MusicController.controller.SetPitch(timeScale);
        }
    }
}
