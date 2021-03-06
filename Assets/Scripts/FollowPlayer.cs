﻿using UnityEngine;

public class FollowPlayer : MonoBehaviour {

	[SerializeField]
	float offSetToFollow;
	
	[SerializeField]
	float smooth;

	Transform player;
	
	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	//void FixedUpdate() {
 //       transform.position = Vector3.Lerp(transform.position, new Vector3(player.position.x + offSetToFollow, 0, transform.position.z), smooth * Time.deltaTime);
 //   }
	
	public void SetOffsetToFollow(float offSet) {
		this.offSetToFollow = offSet;
	}
	
	public void SetSmooth(float smooth) {
		this.smooth = smooth;
	}
}