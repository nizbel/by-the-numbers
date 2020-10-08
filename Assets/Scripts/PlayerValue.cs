using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerValue : MonoBehaviour {

    GameObject playerBlock = null;

    GameObject positiveEnergy = null;
	GameObject negativeEnergy = null;

	// Use this for initialization
	void Start () {
        playerBlock = GameObject.FindGameObjectWithTag("Player");
        positiveEnergy = GameObject.Find("Positive Energy Bar");
		negativeEnergy = GameObject.Find("Negative Energy Bar");
	}
	
	// Update is called once per frame
	void Update () {
        //gameObject.GetComponent<TextMesh>().text = playerBlock.GetComponent<PlayerShip>().GetValue().ToString();
        //if (int.Parse(gameObject.GetComponent<TextMesh>().text) >= 0) {
        //    gameObject.GetComponent<TextMesh>().color = Color.blue;
        //}
        //else {
        //    gameObject.GetComponent<TextMesh>().color = Color.red;
        //}

        float currentValue = playerBlock.GetComponent<PlayerShip>().GetValue();

		if (currentValue > 0) {
			positiveEnergy.GetComponent<Image>().fillAmount = currentValue / StageController.SHIP_VALUE_LIMIT;
			negativeEnergy.GetComponent<Image>().fillAmount = 0;
		} else if (currentValue < 0) {
			positiveEnergy.GetComponent<Image>().fillAmount = 0;
			negativeEnergy.GetComponent<Image>().fillAmount = currentValue / -StageController.SHIP_VALUE_LIMIT;
		} else {
			positiveEnergy.GetComponent<Image>().fillAmount = 0;
			negativeEnergy.GetComponent<Image>().fillAmount = 0;
		}

		// TODO Make this better
		float maxValue = ValueRange.rangeController.GetMaxValue();

		GameObject barMask = GameObject.Find("Energy Bar Mask");
		barMask.transform.localPosition = new Vector3((maxValue - 5) * barMask.GetComponent<RectTransform>().rect.width/30 , barMask.transform.localPosition.y, barMask.transform.localPosition.z);
		foreach (Transform childTransform in barMask.transform) {
			childTransform.localPosition = new Vector3(-barMask.transform.localPosition.x, 0 ,0);
		}
	}
}
