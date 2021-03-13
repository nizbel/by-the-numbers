using UnityEngine;

public class Asteroid : DestructibleObject {
    [SerializeField]
    Vector2 spriteAmount = new Vector2(5, 6);

    void Start() {
        spriteRenderer.material.SetVector("_SpriteAmount", new Vector4(spriteAmount.x, spriteAmount.y, 0, 0));
    }

    public override void OnObjectDespawn() {
        // TODO Workaround for destructible objects list in OutScreenDestroyerController
        FixAddedToList();

        // Remove movement scripts
        RemoveMovementScripts();
        IMovingObject movingScript = GetComponent<IMovingObject>();

        movingScript.enabled = false;

        ObjectPool.SharedInstance.ReturnPooledObject(GetPoolType(), gameObject);
    }

    public void ProcessCollisionBase() {
        // Checks if debris belongs in a formation, if true remove it
        Formation parentFormation = transform.parent.GetComponent<Formation>();
        if (parentFormation != null) {
            transform.parent = parentFormation.transform.parent;

            if (transform == parentFormation.GetCenterElement()) {
                parentFormation.SetCenterElement(null);
            }
            parentFormation.ImpactFormation();
        }

        RemoveMovementScripts();
    }

    // Remove energy movement scripts
    private void RemoveMovementScripts() {
        //IMovingObject movingScript = GetComponent<IMovingObject>();
        //if (movingScript != null) {
        //    Destroy(movingScript);
        //}
        //movingScript.enabled = false;

        RotatingObject rotatingScript = GetComponent<RotatingObject>();
        if (rotatingScript != null) {
            Destroy(rotatingScript);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        ProcessCollisionBase();
    }
}
