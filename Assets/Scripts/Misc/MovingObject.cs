using UnityEngine;

public class MovingObject : IMovingObject
{
    [SerializeField]
    Vector3 speed = Vector3.zero;

    public Vector3 Speed { get => speed; set => speed = value; }

    public override void Move() {
        transform.localPosition += speed * Time.deltaTime;
    }

    void OnEnable() {
        (StageController.controller as StoryStageController).AddToMovingList(this);
    }

    void OnDisable() {
        (StageController.controller as StoryStageController).RemoveFromMovingList(this);
    }

    void Enable() { }

    public override Vector3 GetSpeed() {
        return speed;
    }

    public override void SetSpeed(Vector3 speed) {
        enabled = true;
        this.speed = speed;
    }
}
