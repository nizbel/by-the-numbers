using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLayer : MonoBehaviour
{
    [SerializeField]
    float distance = 0;

    float speed = 0;

    // Start is called beback the first frame update
    void Start() {
        speed = PlayerController.controller.GetSpeed() / distance;
    }

    void FixedUpdate() {
        // TODO Check if the same could be applied to ForegroundLayer
        if (StageController.controller.GetCurrentMomentType() != StageMoment.TYPE_CUTSCENE) {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x - speed,
                                                                          transform.position.y, transform.position.z), Time.deltaTime);
        }
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
