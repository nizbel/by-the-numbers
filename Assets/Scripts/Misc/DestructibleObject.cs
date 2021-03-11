using UnityEngine;
using System;
using System.Collections.Generic;

public class DestructibleObject : MonoBehaviour, IPooledObject {
	// Destructible types
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
	protected int destructibleType = 0;

	// TODO Find a better way to store sprite info
	protected SpriteRenderer spriteRenderer = null;

	// Keeps track of belonging pool in ObjectPool
	private ElementsEnum poolType;

	void Start() {
		OnObjectSpawn();
    }

	// Use this for initialization
	public virtual void OnObjectSpawn() {
		// Define its type if not set
		if (destructibleType == 0) {
			if (GetComponent<SpriteRenderer>() != null) {
				destructibleType = COMMON_SPRITE_TYPE;
				spriteRenderer = GetComponent<SpriteRenderer>();
			}
			else if (GetComponent<Formation>() != null) {
				destructibleType = FORMATION_TYPE;
			}
			else if (GetComponent<MultipleSpriteObject>() != null) {
				DefineMultipleSpriteType();
			}
			else {
				Debug.LogError("INVALID TYPE");
			}
		}
	}

	protected void DefineMultipleSpriteType() {
		destructibleType = MULTIPLE_SPRITE_TYPE;
		spriteRenderer = GetComponent<MultipleSpriteObject>().GetBiggestSpriteRenderer();
	}

	public virtual void OnObjectDespawn() {
		if (poolType == 0) {
			Debug.Log("DESPAWNED " + gameObject.name);
			Destroy(gameObject);
		} else {
			FixAddedToList();
			ObjectPool.SharedInstance.ReturnPooledObject(poolType, gameObject);
        }
	}

	// TODO Workaround for destructible objects list in OutScreenDestroyerController
	protected void FixAddedToList () {
		addedToList = false;
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

	public ElementsEnum GetPoolType() {
		return poolType;
    }

	public void SetPoolType(ElementsEnum poolType) {
		this.poolType = poolType;
    }
}
