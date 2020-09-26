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


    // Start is called before the first frame update
    void Start()
    {
        if (shouldActivate) {
            GetComponent<DirectionalMovingObject>().enabled = true;
            GetComponent<RotatingObject>().enabled = true;
            GetComponent<AudioSource>().enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
