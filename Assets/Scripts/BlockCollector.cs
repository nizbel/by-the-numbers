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

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag == "Block") {
			Destroy(collider.gameObject);
		}
		else if (collider.gameObject.tag == "Range Changer") {
			Destroy(collider.gameObject);
		}
		else if (collider.gameObject.tag == "Power Up") {
			Destroy(collider.gameObject);
		}
	}
}
