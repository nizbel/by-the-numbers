using UnityEngine;
using System.Collections;

public class SpecialEventController : MonoBehaviour {

    private int currentDay;

    private int eventCode;

    public int CurrentDay { get => currentDay; set => currentDay = value; }
    public int EventCode { get => eventCode; set => eventCode = value; }

    private float waitTime = 3;

    /*
	 * Block prefabs
	 */
    public GameObject addBlockPrefab;
    public GameObject subtractBlockPrefab;

    private float randomOffset = 0;

    // Use this for initialization
    void Start() {
        randomOffset = Random.Range(0,1f);
    }

    // Update is called once per frame
    void Update() {
        if (waitTime > 0) {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0) {
                // Spawn wall of energies
                Vector3 position = new Vector3(GameController.GetCameraXMax() + 1, GameController.GetCameraYMax(), 0);
                while (position.y >= GameController.GetCameraYMin()) {
                    randomOffset = (randomOffset * 2) % 1;
                    GameObject chosenPrefab = randomOffset > 0.5f ? addBlockPrefab : subtractBlockPrefab;
                    GameObject newForegroundElement = (GameObject)Instantiate(chosenPrefab, position, new Quaternion(0, 0, 0, 1));
                    newForegroundElement.transform.localRotation = GameObjectUtil.GenerateRandomRotation();

                    position -= new Vector3(0, GameObjectUtil.GetGameObjectVerticalSize(newForegroundElement)*1.25f, 0);
                    newForegroundElement.GetComponent<OperationBlock>().AddDisappearListener(PlayNarrator);
                }

            }
        }
    }

    void PlayNarrator() {
        // TODO fix fixed string
        NarratorController.controller.StartEventSpeech("Day 1 - Latched to bodywork");

        // Destroy game object
        Destroy(gameObject);
    }
}