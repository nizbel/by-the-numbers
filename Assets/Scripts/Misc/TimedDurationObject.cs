using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedDurationObject : MonoBehaviour
{
    float startTime = 0;

    float waitTime = 0;

    float duration = 0;

    // Events
    private UnityEvent onWait = new UnityEvent();

    public float WaitTime { set => waitTime = value; }
    public float Duration { get => duration; set => duration = value; }

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > startTime + Duration && waitTime == 0) {
            Destroy(this.gameObject);
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
