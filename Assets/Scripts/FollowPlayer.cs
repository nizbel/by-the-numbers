using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
	
	float offSetToFollow;
	
	float smooth;
	
	Transform player;
	
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		offSetToFollow = 6f;
		smooth = 10;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void FixedUpdate() {
		transform.position = Vector3.Lerp(transform.position, new Vector3(player.position.x + offSetToFollow, 0, -10), smooth*Time.deltaTime);
	}
	
	public void setOffsetToFollow(float offSet) {
		this.offSetToFollow = offSet;
	}
	
	public void setSmooth(float smooth) {
		this.smooth = smooth;
	}
}