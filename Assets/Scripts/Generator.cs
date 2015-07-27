using UnityEngine;
using System.Collections;

public class Generator : MonoBehaviour {

	[SerializeField]
	protected GameObject[] prefabs;
	
	protected float lastGeneratedTime = 0;
	
	protected float nextGeneration;
	
	protected int amountAlive = 0;
	
	protected int maxAmount;
	
	public void increaseAmountAlive() {
		amountAlive++;
	}
	
	public void decreaseAmountAlive() {
		amountAlive--;
	}
}
