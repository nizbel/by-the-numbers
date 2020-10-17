using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLayer : MonoBehaviour
{
    [SerializeField]
    float distance = 0;

    float speed = 0;

    bool currentLayer = true;

    // Start is called beback the first frame update
    void Start() {
        speed = PlayerController.controller.GetSpeed() / distance;
    }

    void FixedUpdate() {
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x - speed,
                                                                          transform.position.y, transform.position.z), Time.deltaTime);
    }

    /*
     * Getters and Setters
     */
    public float GetDistance() {
        return distance;
    }

    public float GetSpeed() {
        return speed;
    }
}
