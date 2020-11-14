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

    // Update is called once per frame
    void Update()
    {
        //if (GetComponent<BoxCollider2D>().enabled) {
        //    transform.position = Vector3.right * transform.position.x + Vector3.up * PlayerController.controller.transform.position.y;
        //} else if (!GetComponent<AudioSource>().isPlaying) {
        //    gameObject.SetActive(false);
        //}
    }

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag == "Obstacle") {
            GetComponent<AudioSource>().Play();
            PlayerController.controller.ActivateBulletTime();
            GetComponent<BoxCollider2D>().enabled = false;
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
