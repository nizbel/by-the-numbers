using UnityEngine;

public static class GameObjectUtil
{
	public static  float GetGameObjectVerticalSize(GameObject gameObj) {
		return GetGameObjectHalfVerticalSize(gameObj) * 2;
	}

	public static float GetGameObjectHalfVerticalSize(GameObject gameObj) {
		// TODO find a way to get the object's largest sprite
		MultipleSpriteObject multipleSpriteScript = gameObj.GetComponent<MultipleSpriteObject>();
		if (multipleSpriteScript == null) {
			return gameObj.GetComponent<SpriteRenderer>().sprite.bounds.extents.y * gameObj.transform.localScale.y; 
		} else {
			return multipleSpriteScript.GetBiggestSpriteRenderer().sprite.bounds.extents.y * gameObj.transform.localScale.y;
		}
	}

	public static float GetGameObjectHorizontalSize(GameObject gameObj) {
		return GetGameObjectHalfHorizontalSize(gameObj) * 2;
	}

	public static float GetGameObjectHalfHorizontalSize(GameObject gameObj) {
		// TODO find a way to get the object's largest sprite
		MultipleSpriteObject multipleSpriteScript = gameObj.GetComponent<MultipleSpriteObject>();
		if (multipleSpriteScript == null) {
			return gameObj.GetComponent<SpriteRenderer>().sprite.bounds.extents.x * gameObj.transform.localScale.x;
		}
		else {
			return multipleSpriteScript.GetBiggestSpriteRenderer().sprite.bounds.extents.x * gameObj.transform.localScale.x;
		}
	}

	public static Quaternion GenerateRandomRotation() {
		return Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
	}

	public static float GetBiggestSideOfSprite(Sprite sprite) {
		return Mathf.Max(sprite.bounds.extents.x, sprite.bounds.extents.y);
	}

	public static float GetBiggestSideOfSpriteByGameObject(GameObject gameObj) {
		MultipleSpriteObject multipleSpriteScript = gameObj.GetComponent<MultipleSpriteObject>();
		if (multipleSpriteScript == null) {
			return GetBiggestSideOfSprite(gameObj.GetComponent<SpriteRenderer>().sprite);
		}
		else {
			return GetBiggestSideOfSprite(multipleSpriteScript.GetBiggestSpriteRenderer().sprite);
		}
	}

	// TODO Check if it is used
    public static IMovingObject AddMovingScript(GameObject gameObject) {
        // Check if it has a rigid body
        if (gameObject.GetComponent<Rigidbody2D>() != null) {
            return gameObject.AddComponent<MovingRigidBodyObject>();
        }
        else {
            return gameObject.AddComponent<MovingObject>();
        }
    }
}
