using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundLayer : MonoBehaviour
{
    private const int HORIZONTAL_LIMIT = -500;

    float playerSpeed = 0;

    bool currentLayer = true;

    Coroutine refreshMainForegroundLayer = null;

    void Start() {
        playerSpeed = PlayerController.controller.GetSpeed();
        refreshMainForegroundLayer = StartCoroutine(RefreshMainForegroundLayer());
    }

    void Update() {
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x - playerSpeed,
                                                                          transform.position.y, transform.position.z), Time.deltaTime);
    }

    IEnumerator RefreshMainForegroundLayer() {
        float waitTime = (HORIZONTAL_LIMIT / -playerSpeed) / 4;
        while (true) {
            yield return new WaitForSeconds(waitTime);
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
    }

    void OnDestroy() {
        StopCoroutine(refreshMainForegroundLayer);
    }

    public void SetPlayerSpeed(float playerSpeed) {
        this.playerSpeed = playerSpeed;
    }
}
