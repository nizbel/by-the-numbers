using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObjectActivator : MonoBehaviour
{
    private bool shouldActivate = true;
    public bool ShouldActivate { get => shouldActivate; set {
            shouldActivate = value;
            Debug.Log(shouldActivate);
        }
    }

    private float activationDelay = 0;
    public float ActivationDelay { get => activationDelay; set => activationDelay = value; }

    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldActivate && (Time.time - startTime) > activationDelay) {
            Activate();
        }
    }

    private void Activate() {
        GetComponent<DirectionalMovingObject>().enabled = true;
        GetComponent<RotatingObject>().enabled = true;
        GetComponent<AudioSource>().enabled = true;
        Destroy(this);
    }

}
