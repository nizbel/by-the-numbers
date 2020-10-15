using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundLayer : MonoBehaviour
{
    float playerSpeed = 0;

    bool currentLayer = true;

    // Start is called before the first frame update
    void Start()
    {
        StageController.controller.AddForegroundLayer(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x <= -100 && currentLayer) {
            GameObject newLayerObject = new GameObject("Foreground Layer");
            ForegroundLayer foregroundLayerScript = newLayerObject.AddComponent<ForegroundLayer>();

            StageController.controller.AddForegroundLayer(foregroundLayerScript);

            // Mark itself as not the current layer anymore
            currentLayer = false;
        } else if (!currentLayer && transform.childCount == 0) {
            Destroy(gameObject);
        }

    }


    void FixedUpdate() {
        if (playerSpeed == 0) {
            playerSpeed = StageController.controller.GetPlayerShipSpeed();
        }
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x - playerSpeed,
                                                                          transform.position.y, transform.position.z), Time.deltaTime);
    }
}
