using UnityEngine;

public static class GameObjectUtil
{

	public static  float GetGameObjectVerticalSize(GameObject gameObj) {
		// TODO find a way to get the object's largest sprite
		//return gameObj.GetComponent<SpriteRenderer>().sprite.bounds.extents.y * 2 * gameObj.transform.localScale.y;
		return GetGameObjectHalfVerticalSize(gameObj) * 2;
	}

	public static float GetGameObjectHalfVerticalSize(GameObject gameObj) {
		// TODO find a way to get the object's largest sprite
		return gameObj.GetComponent<SpriteRenderer>().sprite.bounds.extents.y * gameObj.transform.localScale.y;
	}

	public static Quaternion GenerateRandomRotation() {
		return Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f));

	}
}
