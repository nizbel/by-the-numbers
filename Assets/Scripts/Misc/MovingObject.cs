using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public const float MIN_FOREGROUND_ELEMENT_SPEED_X = -2.25f;
    public const float MAX_FOREGROUND_ELEMENT_SPEED_X = 1.5f;

    [SerializeField]
    Vector3 speed = Vector3.zero;

    Rigidbody2D movingRigidBody;

    // Start is called before the first frame update
    void Start()
    {
        movingRigidBody = GetComponent<Rigidbody2D>();
        movingRigidBody.velocity = speed;
    }

    public Vector3 Speed { get => speed; 
        set { 
            speed = value;
            if (movingRigidBody != null) {
                movingRigidBody.velocity = speed;
            }
        } 
    }
}
