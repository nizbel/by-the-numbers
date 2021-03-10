using UnityEngine;

public class MovingRigidBodyObject : IMovingObject
{
    Rigidbody2D movingRigidBody;

    Vector3 oldPosition;

    void Awake() {
        movingRigidBody = GetComponent<Rigidbody2D>();
    }

    void Start() {
        // Start position tracking
        oldPosition = transform.localPosition;
    }

    public override void Move() {
        transform.localPosition = oldPosition + (Vector3)movingRigidBody.velocity * Time.deltaTime;

        oldPosition = transform.localPosition;
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

    public override Vector3 GetSpeed() {
        return movingRigidBody.velocity;
    }

    public override void SetSpeed(Vector3 speed) {
        enabled = true;
        movingRigidBody.velocity = speed;
    }
}
