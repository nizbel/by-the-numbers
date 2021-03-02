using UnityEngine;

public class DirectionalMovingObject : IMovingObject {
    [SerializeField]
    float speed = 1.5f;

    [SerializeField]
    float offsetAngle = 0;

    Rigidbody2D movingRigidBody;

    Vector3 oldPosition;

    bool collided = false;

    void Awake() {
        movingRigidBody = GetComponent<Rigidbody2D>();
    }

    void Start() {
        // Start position tracking
        oldPosition = transform.localPosition;
    }

    public override void Move() {
        if (collided) {
            transform.localPosition = oldPosition + (Vector3)movingRigidBody.velocity * Time.deltaTime;
        }
        else {
            transform.localPosition = oldPosition + GetSpeed() * Time.deltaTime;
        }
        oldPosition = transform.localPosition;
    }

    public override Vector3 GetSpeed() {
        float directionAngle = transform.localRotation.eulerAngles.z + offsetAngle;

        float speedX = speed * Mathf.Cos(directionAngle * Mathf.Deg2Rad);
        float speedY = speed * Mathf.Sin(directionAngle * Mathf.Deg2Rad);

        // TODO Test if this works
        // Set rigid body velocity every time speed changes
        movingRigidBody.velocity = new Vector3(speedX, speedY, 0);

        return movingRigidBody.velocity;
    }

    public override void SetSpeed(Vector3 speed) {
        this.speed = speed.magnitude;
        movingRigidBody.velocity = speed;
    }

    void OnEnable() {
        // TODO Change to use a general stage controller
        (StageController.controller as StoryStageController).AddToMovingList(this);
        // Start position tracking
        oldPosition = transform.localPosition;
    }

    void OnDisable() {
        (StageController.controller as StoryStageController).RemoveFromMovingList(this);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        collided = true;
    }
}
