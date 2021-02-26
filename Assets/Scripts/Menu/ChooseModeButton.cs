using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ChooseModeButton : MonoBehaviour, IPointerEnterHandler {

	public const int STORY_MODE = 1;
	public const int INFINITE_MODE = 2;

	[SerializeField]
	private int mode;

	[SerializeField]
	private string modeDescription;

	[SerializeField]
	private TextMeshProUGUI modeDescriptionText;

	public void SelectMode() {
		if (mode == STORY_MODE) {
            GameController.controller.SetCurrentDay(CurrentDayController.GetInitialDay());
            GameController.controller.ChangeState(GameController.GAMEPLAY_STORY);
		} else {
			GameController.controller.ChangeState(GameController.GAMEPLAY_INFINITE);
		}
	}
	
 
     // When highlighted with mouse.
    public void OnPointerEnter(PointerEventData eventData) {
		modeDescriptionText.text = modeDescription;
	}

    /*
	 * Getters and Setters
	 */
    public int GetMode() {
		return mode;
	}
}
