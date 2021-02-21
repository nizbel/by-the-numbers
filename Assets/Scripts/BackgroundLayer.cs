using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLayer : MonoBehaviour
{
    [SerializeField]
    float distance = 0;

    float speed = 0;

    [SerializeField]
    LayerMask layerMask;
    int layer;

    [SerializeField]
    string spriteLayerName;

    // Start is called beback the first frame update
    void Start() {
        //speed = PlayerController.controller.GetSpeed() / distance;
        speed = PlayerController.controller.GetSpeed() * Mathf.Pow(2, 2.5f - Mathf.Sqrt(distance));
        //Debug.Log(gameObject.name + " = " + (speed / PlayerController.controller.GetSpeed()));
        layer = (int) Mathf.Log(layerMask.value, 2);
    }

    void Update() {
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

    public LayerMask GetLayer() {
        return layer;
    }

    public string GetSpriteLayerName() {
        return spriteLayerName;
    }
}
