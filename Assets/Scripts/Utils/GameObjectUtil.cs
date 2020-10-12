﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectUtil
{

	public static  float GetGameObjectVerticalSize(GameObject gameObj) {
		// TODO find a way to get the object's largest sprite
		return gameObj.GetComponent<SpriteRenderer>().sprite.bounds.extents.y * 2 * gameObj.transform.localScale.y;
	}
}