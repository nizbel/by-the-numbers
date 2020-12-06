using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedDurationObject : MonoBehaviour
{
    float startTime = 0;

    [SerializeField]
    float waitTime = 0;

    [SerializeField]
    float duration = 0;

    // Checks if object should be destroyed or deactivated
    [SerializeField]
    bool shouldDestroy = true;

    // Events
    private UnityEvent onWait = new UnityEvent();

    public float WaitTime { set => waitTime = value; }
    public float Duration { get => duration; set => duration = value; }

    void OnEnable() {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > startTime + Duration && waitTime == 0) {
            if (shouldDestroy) {
                Destroy(this.gameObject);
            } else {
                this.gameObject.SetActive(false);
            }
        } else if (waitTime > 0 && Time.time > startTime + waitTime) {
            // Start after waiting
            waitTime = 0;
            startTime = Time.time;
            onWait.Invoke();
        }
    }

    public void AddOnWaitListener(UnityAction action) {
        onWait.AddListener(action);
    }
}
