using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterPosition : MonoBehaviour
{
    private const float DEFAULT_X_REMOVAL_POSITION = -100f;
    public enum ThresholdTestEnum {
        Disabled,
        Higher,
        Lower
    }

    ThresholdTestEnum thresholdXType = ThresholdTestEnum.Disabled;
    float limitX;

    ThresholdTestEnum thresholdYType = ThresholdTestEnum.Disabled;
    float limitY;

    void FixedUpdate()
    {
        if (PassedAllPositions()) {
            Debug.Log(transform.position + "..." + limitY);

            // Move to a place where object will be removed and disable its movement script
            transform.position = new Vector3(DEFAULT_X_REMOVAL_POSITION, 0, 0);
            GetComponent<IMovingObject>().enabled = false;

            // Checks if belongs in a formation, if true remove it
            Formation parentFormation = transform.parent.GetComponent<Formation>();
            if (parentFormation != null) {
                transform.parent = parentFormation.transform.parent;
            }

            Destroy(this);
        }
    }

    void OnDisable() {
        Destroy(this);
    }

    bool PassedAllPositions() {
        return PassedX() && PassedY();
    }

    bool PassedX() {
        switch (thresholdXType) {
            case ThresholdTestEnum.Higher:
                return transform.position.x >= limitX;

            case ThresholdTestEnum.Lower:
                return transform.position.x <= limitX;

            default:
                return true;
        }
    }

    bool PassedY() {
        switch (thresholdYType) {
            case ThresholdTestEnum.Higher:
                return transform.position.y >= limitY;

            case ThresholdTestEnum.Lower:
                return transform.position.y <= limitY;

            default:
                return true;
        }
    }

    // Getters and Setters
    public void SetThresholdXType(ThresholdTestEnum thresholdXType) {
        this.thresholdXType = thresholdXType;
    }

    public void SetLimitX(float limitX) {
        this.limitX = limitX;
    }

    public void SetThresholdYType(ThresholdTestEnum thresholdYType) {
        this.thresholdYType = thresholdYType;
    }
    public void SetLimitY(float limitY) {
        this.limitY = limitY;
    }
}
