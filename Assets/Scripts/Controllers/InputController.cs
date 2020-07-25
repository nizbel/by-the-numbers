﻿using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

	PlayerController playerController;

	// Use this for initialization
	void Start () {
		playerController = GameObject.FindGameObjectWithTag("Player Controller").GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
		if ((Input.GetMouseButtonDown(0)) || (Input.touchCount > 0)) {
			Vector3 hitPosition = Vector3.zero;
			RaycastHit2D[] hits = new RaycastHit2D[1];
			switch (Application.platform) {
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.WindowsPlayer:
				hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				hitPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				break;
			case RuntimePlatform.Android:
				hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
				hitPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
				break;
			}
			foreach (RaycastHit2D hitOrig in hits)
			{
//				if (hitOrig.collider.tag == "Block Controller") {
//					hitOrig.collider.gameObject.GetComponent<BlockController>().setBlockPosition(hitPosition.y);
//				}
			}
			playerController.SetBlockPosition(hitPosition.y);
		}
	}
}
