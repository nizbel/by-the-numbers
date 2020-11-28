using UnityEngine;
using System;

public class DestructibleObject : MonoBehaviour {
	public const int COMMON_SPRITE_TYPE = 1;
	public const int FORMATION_TYPE = 2;
	public const int MULTIPLE_SPRITE_TYPE = 3;

	// Keeps track of object's speed in order to prepare destructible ordered list
	private float speed;

	// Checks if already added into destructible's list
	private Boolean addedToList = false;

	// Guarantees the object can be destroyed
	private Boolean destructibleNow = true;

	// Keeps track of its type for destruction verification
	private int destructibleType = 0;

	// TODO Find a better way to store sprite info
	private SpriteRenderer spriteRenderer = null;

	// Use this for initialization
	void Start () {
		// Define its type
		if (GetComponent<SpriteRenderer>() != null) {
			destructibleType = COMMON_SPRITE_TYPE;
			spriteRenderer = GetComponent<SpriteRenderer>();
        } else if (GetComponent<Formation>() != null) {
			destructibleType = FORMATION_TYPE;
		} else if (GetComponent<MultipleSpriteObject>() != null) {
			destructibleType = MULTIPLE_SPRITE_TYPE;
			spriteRenderer = GetComponent<MultipleSpriteObject>().GetBiggestSpriteRenderer();
		} else {
			Debug.LogError("INVALID TYPE");
        }
	}

    public float GetSpeed() {
        return speed;
    }

	public void SetSpeed(float speed) {
		this.speed = speed;
		if (!addedToList) {
			OutScreenDestroyerController.controller.AddToDestructibleList(this);
			addedToList = true;
		}
	}

	public bool IsDestructibleNow() {
		return destructibleNow;
	}

	public void SetIsDestructibleNow(bool destructibleNow) {
		this.destructibleNow = destructibleNow;
	}

	public int GetDestructibleType() {
		return destructibleType;
	}

	public SpriteRenderer GetSpriteRenderer() {
		return spriteRenderer;
    }
}
