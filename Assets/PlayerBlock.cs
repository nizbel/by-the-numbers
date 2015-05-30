using UnityEngine;
using System.Collections;

public class PlayerBlock : MonoBehaviour {

	public int value = 0;

	float speed = 4;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {	
		transform.Translate(Vector3.right * speed * Time.deltaTime);
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.tag == "Block") {
			Debug.Log("Collided with block");
			value = collision.gameObject.GetComponent<OperationBlock>().operation(value);
			Destroy(collision.gameObject);
		}
	}
}
