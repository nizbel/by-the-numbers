using UnityEngine;

public class ForegroundElement : MonoBehaviour {
    public const float MIN_FOREGROUND_ELEMENT_SPEED_X = -2.25f;
    public const float MAX_FOREGROUND_ELEMENT_SPEED_X = 1.5f;

    DestructibleObject destructibleScript = null;

    // OnEnable can be used after Start set up its variables
    void OnEnable() {
        if (destructibleScript != null) {
            // TODO Decide about speeds, whether it should be decided here or not
            if (transform.GetComponent<Formation>() != null) {
                destructibleScript.SetSpeed(PlayerController.controller.GetSpeed());

                // Do the same for every child
                foreach (Transform child in transform) {
                    child.GetComponent<DestructibleObject>().SetSpeed(PlayerController.controller.GetSpeed());
                }
            }
            else {
                // Default case, every foreground element is destructible
                destructibleScript.SetSpeed(PlayerController.controller.GetSpeed());
            }

            // Register itself to the current foreground layer
            if (transform.parent == null || transform.parent == ObjectPool.SharedInstance.transform) {
                transform.parent = StageController.controller.GetCurrentForegroundLayer().transform;
            }
        }
    }

    // Start is called before the first frame update
    void Start() {
        destructibleScript = GetComponent<DestructibleObject>();

        // Allow OnEnable to act the first time
        OnEnable();
    }
}
