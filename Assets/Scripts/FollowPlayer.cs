using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

	[SerializeField]
	float offSetToFollow;
	
	[SerializeField]
	float smooth;

	Transform player;

	// The script that controls player movement
	PlayerShip playerBlock;
	
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		playerBlock = player.GetComponent<PlayerShip>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void FixedUpdate() {
		transform.position = Vector3.Lerp(transform.position, new Vector3(player.position.x + offSetToFollow, 0, transform.position.z), smooth*Time.deltaTime);
	}
	
	public void setOffsetToFollow(float offSet) {
		this.offSetToFollow = offSet;
	}
	
	public void setSmooth(float smooth) {
		this.smooth = smooth;
	}
}