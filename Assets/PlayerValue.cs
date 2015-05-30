using UnityEngine;
using System.Collections;

public class PlayerValue : MonoBehaviour {
	
	GameObject playerBlock;
	
	// Use this for initialization
	void Start () {
		playerBlock = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.GetComponent<TextMesh>().text = playerBlock.GetComponent<PlayerBlock>().value.ToString();
		if (int.Parse(gameObject.GetComponent<TextMesh>().text) >= 0) {
			gameObject.GetComponent<TextMesh>().color = Color.blue;
		}
		else {
			gameObject.GetComponent<TextMesh>().color = Color.red;
		}
	}
}
