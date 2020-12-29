using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundLayer : MonoBehaviour
{
    private const int HORIZONTAL_LIMIT = -500;

    float playerSpeed = 0;

    bool currentLayer = true;

    void Awake()
    {
        StageController.controller.AddForegroundLayer(this);
        playerSpeed = PlayerController.controller.GetSpeed();
    }

    void FixedUpdate() {
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x - playerSpeed,
                                                                          transform.position.y, transform.position.z), Time.deltaTime);
        
        // Destroy game object once it reaches its limit, creating another to replace
        if (transform.position.x <= HORIZONTAL_LIMIT && currentLayer) {
            GameObject newLayerObject = new GameObject("Foreground Layer");
            ForegroundLayer foregroundLayerScript = newLayerObject.AddComponent<ForegroundLayer>();

            // Mark itself as not the current layer anymore
            currentLayer = false;
        }
        else if (!currentLayer && transform.childCount == 0) {
            Destroy(gameObject);
        }
    }

    public void SetPlayerSpeed(float playerSpeed) {
        this.playerSpeed = playerSpeed;
    }
}
