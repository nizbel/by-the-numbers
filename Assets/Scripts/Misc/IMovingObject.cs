using UnityEngine;

// Class used to control objects movement, especially ones with rigid bodies
public abstract class IMovingObject : MonoBehaviour
{
    public abstract void Move();
    public abstract Vector3 GetSpeed();

    public abstract void SetSpeed(Vector3 speed);

}
