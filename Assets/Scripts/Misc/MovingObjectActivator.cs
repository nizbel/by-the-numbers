using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO rethink about the usage of this class
public class MovingObjectActivator : MonoBehaviour
{
    //private bool shouldActivate = true;
    //public bool ShouldActivate { get => shouldActivate; set {
    //        shouldActivate = value;
    //    }
    //}

    private float activationDelay = 0;
    public float ActivationDelay { get => activationDelay; set => activationDelay = value; }

    private float startTime;

    private bool stopShaking = true;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime > activationDelay) {
            Activate();
        }
    }

    private void Activate() {
        // Check which scripts to activate
        if (GetComponent<DirectionalMovingObject>() != null) {
            GetComponent<DirectionalMovingObject>().enabled = true;
        }
        if (GetComponent<RotatingObject>() != null) {
            GetComponent<RotatingObject>().enabled = true;
        }

        // Check if should remove shaking
        if (stopShaking && GetComponent<ShakyObject>() != null) {
            Destroy(GetComponent<ShakyObject>());
        }
        Destroy(this);
    }

}
