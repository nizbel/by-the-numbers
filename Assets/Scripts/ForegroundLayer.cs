using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundLayer : MonoBehaviour
{
    private const int HORIZONTAL_LIMIT = -500;

    float playerSpeed = 0;

    bool currentLayer = true;

    // Start is called before the first frame update
    void Start()
    {
        StageController.controller.AddForegroundLayer(this);
        playerSpeed = PlayerController.controller.GetSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        // Destroy game object once it reaches its limit, creating another to replace
        if (transform.position.x <= HORIZONTAL_LIMIT && currentLayer) {
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
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x - playerSpeed,
                                                                          transform.position.y, transform.position.z), Time.deltaTime);
    }
}
