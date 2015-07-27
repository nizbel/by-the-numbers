using UnityEngine;
using System.Collections;

public class TextRenderLayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// TODO set layer rendering order
		renderer.sortingLayerName = "Interface";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
