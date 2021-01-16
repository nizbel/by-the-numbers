using UnityEngine;
using System.Collections;

public class SpecialEventDebrisController : MonoBehaviour {

    private const int INITIAL_STATE = 0;
    private const int SEEN_STATE = 1;
    private const int PAST_STATE = 2;

    private int eventCode;

    public int EventCode { get => eventCode; set => eventCode = value; }

    private int state = INITIAL_STATE;

    private GameObject debris = null;

    // Use this for initialization
    void Start() {
        // Spawn debris
        Vector2 position = new Vector2(GameController.GetCameraXMax() + 1, PlayerController.controller.transform.position.y);
        debris = ObjectPool.SharedInstance.SpawnPooledObject(ObjectPool.DEBRIS, position, GameObjectUtil.GenerateRandomRotation());
    }

    // Update is called once per frame
    void Update() {
        switch (state) {
            case INITIAL_STATE:
                if (debris.transform.position.x < GameController.GetCameraXMax()) {
                    ObserveDebris();
                }
                break;
            case SEEN_STATE:
                if (!debris.activeSelf) {
                    SpeakAboutElement();
                }
                break;
            case PAST_STATE:
                if (NarratorController.controller.GetState() != NarratorController.IMPORTANT) {
                    Destroy(gameObject);
                }
                break;
        }
    }

    void ObserveDebris() {
        // TODO Play observation remark
        //NarratorController.controller.StartMomentSpeech("");

        state = SEEN_STATE;
    }

    void SpeakAboutElement() {
        // TODO Play observation remark
        //NarratorController.controller.StartMomentSpeech("");

        state = PAST_STATE;
    }
}