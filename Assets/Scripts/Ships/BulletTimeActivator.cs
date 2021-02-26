using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTimeActivator : MonoBehaviour
{
    Vector3 startingPosition;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.localPosition;
    }

	void OnTriggerEnter2D(Collider2D collider) {
        switch (collider.tag) {
            case "Obstacle":
            case "Frail Obstacle":
            case "Indestructible Obstacle":
            case "Mine":
            case "Lightning Fuse":
                if (!PlayerController.controller.BulletTimeAvailable()) {
                    // If bullet time is not available, stop
                    return;
                }
                GetComponent<AudioSource>().Play();
                PlayerController.controller.ActivateBulletTime();
                gameObject.SetActive(false);
                break;
        }
	}

    public void SetMoving(bool moving) {
        if (moving) {
            transform.localPosition = startingPosition + Vector3.left * 0.2f;
        } else {
            transform.localPosition = startingPosition;
        }
    }
}
