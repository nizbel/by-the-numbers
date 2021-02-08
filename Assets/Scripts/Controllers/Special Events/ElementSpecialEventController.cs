using UnityEngine;
using System.Collections;

public class ElementSpecialEventController : MonoBehaviour {

    protected const int INITIAL_STATE = 0;
    protected const int SEEN_STATE = 1;
    protected const int PAST_STATE = 2;

    protected int state = INITIAL_STATE;

    protected float waitTime = 1;

    protected GameObject observableElement = null;

    protected ElementEncounterStageMoment stageMoment = null;

    protected ElementsEnum elementType;

    // Speeches
    [SerializeField]
    protected Speech observeSpeech;
    [SerializeField]
    protected Speech aboutElementSpeech;

    // Update is called once per frame
    void Update() {
        if (waitTime > 0) {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0) {
                SpawnElement();
            }
        }
        else {
            switch (state) {
                case INITIAL_STATE:
                    if (observableElement.transform.position.x < GameController.GetCameraXMax()) {
                        state = SEEN_STATE;
                    }
                    break;
                case SEEN_STATE:
                    if (!observableElement.activeSelf && NarratorController.controller.GetState() != NarratorController.IMPORTANT) {
                        SpeakAboutElement();
                    }
                    break;
                case PAST_STATE:
                    if (NarratorController.controller.GetState() != NarratorController.IMPORTANT) {
                        // End moment
                        EndStageMoment();

                        // Save element as seen
                        UpdateElementsSeen();

                        Destroy(gameObject);
                    }
                    break;
            }
        }
    }

    protected virtual void SpawnElement() {
    
    }

    protected void UpdateElementsSeen() {
        GameController.GetGameInfo().elementsSeen[elementType] = true;
        GameController.controller.Save();
    }

    protected void EndStageMoment() {
        stageMoment.duration = 0;
    }

    protected void ObserveElement() {
        // TODO Play observation remark
        NarratorController.controller.StartMomentSpeech(observeSpeech);

        //state = SEEN_STATE;
    }

    protected void SpeakAboutElement() {
        // TODO Play about remark
        NarratorController.controller.StartMomentSpeech(aboutElementSpeech);

        state = PAST_STATE;
    }

    public void SetStageMoment(ElementEncounterStageMoment stageMoment) {
        this.stageMoment = stageMoment;
    }
}