using UnityEngine;
using System.Collections;

public class SpecialEventController : MonoBehaviour {
    
    private const int CURRENT_DAY = 1;

    private const float SIZE_ADJUSTMENT = 1.2f;

    private int eventCode;

    public int EventCode { get => eventCode; set => eventCode = value; }

    private float waitTime = 1;

    /*
	 * Block prefabs
	 */
    public GameObject addBlockPrefab;
    public GameObject subtractBlockPrefab;

    private float randomOffset = 0;

    private bool done = false;

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
                Vector3 position = new Vector3(GameController.GetCameraXMax() + 1, GameController.GetCameraYMax() + 0.5f, 0);
                while (position.y >= GameController.GetCameraYMin()) {
                    // Choose prefab at random
                    randomOffset = (randomOffset * 2) % 1;
                    GameObject chosenPrefab = randomOffset > 0.5f ? addBlockPrefab : subtractBlockPrefab;

                    // Instatiate
                    GameObject newForegroundElement = (GameObject)Instantiate(chosenPrefab, position, new Quaternion(0, 0, 0, 1));
                    newForegroundElement.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 360*randomOffset);

                    // Set position in Y axis near half object's height
                    float halfHeight = GameObjectUtil.GetGameObjectHalfVerticalSize(newForegroundElement) * SIZE_ADJUSTMENT;
                    newForegroundElement.transform.position += Vector3.down * halfHeight;

                    position -= new Vector3(0, halfHeight*2, 0);
                    newForegroundElement.GetComponent<Energy>().AddDisappearListener(PlayNarrator);
                }
            }
        } else if (done) {
            // Check if speech is over, so object can be destroyed
            if (NarratorController.controller.GetState() != NarratorController.IMPORTANT) {
                if (!GameController.GetGameInfo().StagePlayed(CURRENT_DAY)) {
                    // TODO fix fixed string
                    NarratorController.controller.StartMomentSpeech("Day 1 - Positive negative");
                } 
                //else {
                    //// Skip current event to get on with the rest of the stage
                    //StageController.controller.SkipCurrentEvent();
                //}
                Destroy(gameObject);
            }
        }
    }

    void PlayNarrator() {
        if (!done) {
            if (!GameController.GetGameInfo().StagePlayed(CURRENT_DAY)) {
                // TODO fix fixed string
                NarratorController.controller.StartMomentSpeech("Day 1 - Latched to bodywork");
            }
            done = true;
        }
    }
}