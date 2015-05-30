using UnityEngine;
using System.Collections;

public class BlockCollector : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate() {
		transform.position = Vector3.Lerp(transform.position, new Vector3(Camera.main.transform.position.x - 16, 0, 0), 2*Time.deltaTime);
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.tag == "Block") {
			Destroy(collision.gameObject);
		}
		else if (collision.gameObject.tag == "Range Changer") {
			Destroy(collision.gameObject);
		}
	}
}
