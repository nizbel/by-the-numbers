using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedDurationObject : MonoBehaviour
{
    Coroutine startCoroutine;

    [SerializeField]
    float waitTime = 0;

    [SerializeField]
    float duration = 0;

    [SerializeField]
    bool activateOnEnable = false;

    // Checks if object should be destroyed or deactivated
    [SerializeField]
    bool shouldDestroy = true;

    // Events
    private UnityEvent onWait = new UnityEvent();

    public float WaitTime { set => waitTime = value; }
    public float Duration { get => duration; set => duration = value; }

    void OnEnable() {
        if (activateOnEnable) {
            Activate();
        }
    }

    public void Activate() {
        startCoroutine = StartCoroutine(StartAfterWait());
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (Time.time > startTime + Duration && waitTime == 0) {
    //        if (shouldDestroy) {
    //            Destroy(this.gameObject);
    //        } else {
    //            this.gameObject.SetActive(false);
    //        }
    //    } else if (waitTime > 0 && Time.time > startTime + waitTime) {
    //        // Start after waiting
    //        waitTime = 0;
    //        startTime = Time.time;
    //        onWait.Invoke();
    //    }
    //}

    IEnumerator StartAfterWait() {
        yield return new WaitForSeconds(waitTime);
        onWait.Invoke();
        StartCoroutine(EndAfterDuration());
    }

    IEnumerator EndAfterDuration() {
        yield return new WaitForSeconds(duration);
        if (shouldDestroy) {
            Destroy(this.gameObject);
        }
        else {
            this.gameObject.SetActive(false);
        }
    }

    public void AddOnWaitListener(UnityAction action) {
        onWait.AddListener(action);
    }
}
