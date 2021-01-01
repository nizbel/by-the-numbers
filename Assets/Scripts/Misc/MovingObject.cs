using UnityEngine;

public class MovingObject : MonoBehaviour
{
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
