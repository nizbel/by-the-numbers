using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleRemover : MonoBehaviour
{
    private const float DEFAULT_SIZE = 3f;
    private const float DEFAULT_DEACTIVATION_TIMER = 0.8f;

    private float targetPositionY;

    private BoxCollider2D removerCollider;

    private float deactivationTimer = DEFAULT_DEACTIVATION_TIMER;

    // Start is called before the first frame update
    void Start()
    {
        removerCollider = GetComponent<BoxCollider2D>();
        // Become double the size of player's ship
        removerCollider.size = PlayerController.controller.GetSpaceship().GetComponent<BoxCollider2D>().size * DEFAULT_SIZE;

        gameObject.SetActive(false);
    }

    void OnEnable() {
        if (removerCollider != null) {
            transform.position = new Vector3(GameController.GetCameraXMax() + removerCollider.size.x / 2, PlayerController.controller.transform.position.y, 0);

            // Define target position
            targetPositionY = Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax());

            deactivationTimer = DEFAULT_DEACTIVATION_TIMER;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        switch (collision.tag) {
            case "Obstacle":
            case "Frail Obstacle":
                collision.gameObject.transform.position = new Vector3(GameController.GetCameraXMin() - 20, 0, 0);
                break;
        }
    }

    private void FixedUpdate() {
        deactivationTimer -= Time.deltaTime;
        if (deactivationTimer <= 0) {
            gameObject.SetActive(false);
        }

        if (transform.position.y != targetPositionY) {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, targetPositionY, 0), Time.deltaTime * 3);
        } else {
            // Define target position
            targetPositionY = Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax());
        }
    }

    public void ResetDeactivationTimer() {
        deactivationTimer = DEFAULT_DEACTIVATION_TIMER;
    }
}
