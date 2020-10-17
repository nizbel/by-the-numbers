using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RadialFormation : Formation
{
    private const float MAX_RADIUS_SIZE = 2.5f;
    private const float MIN_RADIUS_SIZE = 1;
    private const float MAX_IN_OUT_SPEED = 0.4f;
    private const float MIN_IN_OUT_SPEED = 0.8f;

    float radiusFactor = 0;

    [SerializeField]
    bool doubleDecker = false;

    // Start is called before the first frame update
    void Start()
    {
        radiusFactor = Random.Range(MIN_RADIUS_SIZE, MAX_RADIUS_SIZE);

        // Check if energies will be moving in/out or not
        bool movingEnergies = GameController.RollChance(30);


        // Apply radius to children
        if (movingEnergies) {
            float movementSpeedFactor = Random.Range(MIN_IN_OUT_SPEED, MAX_IN_OUT_SPEED);
            if (doubleDecker) {
                movementSpeedFactor /= 2;
            }

            foreach (Transform child in transform) {
                // Not for child at center
                if (child.localPosition.x != 0 && child.localPosition.y != 0) {
                    RadialInOutMovement movScript = child.gameObject.AddComponent<RadialInOutMovement>();

                    // Define attributes of the in/out movement
                    movScript.InnerPosition = child.localPosition;

                    // Define outer position (max position for the movement)
                    child.localPosition *= radiusFactor;

                    movScript.MovementSpeed = movementSpeedFactor * child.localPosition;
                    movScript.OuterPosition = child.localPosition;

                }
            }
        }
        else {
            foreach (Transform child in transform) {
                // Not for child at center
                if (child.localPosition.x != 0 && child.localPosition.y != 0) {
                    // Just define positions
                    child.localPosition *= radiusFactor;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override float GetScreenOffset() {
        // TODO prepare for double decker
        if (doubleDecker) {
            return MAX_RADIUS_SIZE * 2 + GameObjectUtil.GetGameObjectVerticalSize(gameObject.transform.GetChild(0).gameObject);
        }
        return MAX_RADIUS_SIZE + GameObjectUtil.GetGameObjectVerticalSize(gameObject.transform.GetChild(0).gameObject);
    }
}
