using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {

	// Power Up type
	[SerializeField]
	int type;

	/*
	 * Getters and Setters
	 */
	public int getType() {
		return type;
	}

	public void setType(int type) {
		this.type = type;
	}
}
