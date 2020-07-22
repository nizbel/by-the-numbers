using UnityEngine;
using System.Collections;

public class BlockController : MonoBehaviour {

	float blockPosition = 0;

	Transform player;

	Transform blockBallPosition;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		blockBallPosition = transform.GetChild(0).transform;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate() {
        blockBallPosition.position = Vector3.Lerp(blockBallPosition.position, new Vector3(blockBallPosition.position.x, blockPosition, 0), 1.5f * Time.deltaTime);
        player.position = Vector3.Lerp(player.position, new Vector3(player.position.x, blockPosition, 0), 1.5f * Time.deltaTime);
    }

	public void SetBlockPosition(float blockPosition) {
		this.blockPosition = blockPosition;
	}
}
