using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public const float MIN_FOREGROUND_ELEMENT_SPEED_X = -2.25f;
    public const float MAX_FOREGROUND_ELEMENT_SPEED_X = 1.5f;

    [SerializeField]
    Vector3 initialSpeed = Vector3.zero;

    Rigidbody2D movingRigidBody;

    Vector3 oldPosition;

    // Start is called before the first frame update
    void Start()
    {
        movingRigidBody = GetComponent<Rigidbody2D>();

        if (initialSpeed != Vector3.zero) {
            movingRigidBody.velocity = initialSpeed;
        }

        // Start position tracking
        oldPosition = transform.position;
    }

    // Updates the movement ignoring the update on fixedUpdate by the physics 2d
    void Update() {
        transform.position = oldPosition + (Vector3) movingRigidBody.velocity * Time.deltaTime;
        oldPosition = transform.position;
    }

    public Vector3 Speed { get => initialSpeed; 
        set { 
            initialSpeed = value;
            // TODO Check if this can be removed
            if (movingRigidBody != null) {
                movingRigidBody.velocity = initialSpeed;
            }
        } 
    }
}
