using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private const float VERTICAL_SPEED = 2.5f;

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
        blockBallPosition.position = Vector3.Lerp(blockBallPosition.position, new Vector3(blockBallPosition.position.x, blockPosition, 0), VERTICAL_SPEED * Time.deltaTime);
        player.position = Vector3.Lerp(player.position, new Vector3(player.position.x, blockPosition, 0), VERTICAL_SPEED * Time.deltaTime);
    }

	public void SetBlockPosition(float blockPosition) {
		// Limit block position
		blockPosition = LimitBlockPosition(blockPosition);
		this.blockPosition = blockPosition;
	}

	private float LimitBlockPosition(float blockPosition) {
		if (blockPosition + player.GetComponent<SpriteRenderer>().sprite.bounds.extents.y > GameController.GetCameraYMax()) {
			return GameController.GetCameraYMax() - player.GetComponent<SpriteRenderer>().sprite.bounds.extents.y;
		} else if (blockPosition - player.GetComponent<SpriteRenderer>().sprite.bounds.extents.y < GameController.GetCameraYMin()) {
			return GameController.GetCameraYMin() + player.GetComponent<SpriteRenderer>().sprite.bounds.extents.y;
		}
		return blockPosition;
	}
}
