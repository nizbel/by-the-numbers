using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTimeActivator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<BoxCollider2D>().enabled) {
            transform.position = Vector3.right * transform.position.x + Vector3.up * PlayerController.controller.transform.position.y;
        } else if (!GetComponent<AudioSource>().isPlaying) {
            gameObject.SetActive(false);
        }
    }

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag == "Obstacle") {
            PlayerController.controller.ActivateBulletTime();
            GetComponent<AudioSource>().Play();
            GetComponent<BoxCollider2D>().enabled = false;

        }
	}
}
