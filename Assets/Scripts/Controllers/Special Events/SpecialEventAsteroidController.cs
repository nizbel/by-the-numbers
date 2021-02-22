using UnityEngine;

public class SpecialEventAsteroidController : ElementSpecialEventController {

    private const int ASTEROID_AMOUNT = 4;

    private const float DISTANCE_CROSSING_ASTEROIDS = 3.5f;

    private const float BASE_SPEED_Y = 5f;
    private const float SPEED_DAMPENING_FACTOR = 0.3f;

    void Start() {
        // Define element and speeches
        elementType = ElementsEnum.ASTEROID;

        // Increases stage moment's duration to last as much as necessary
        stageMoment.duration = 60;

        // Increase wait time
        waitTime = observeSpeech.audio.length;

        ObserveElement();
    }

    protected override void SpawnElement() {

        // Spawn initial asteroids
        for (int i = 0; i < ASTEROID_AMOUNT; i++) {
            Vector2 crossPositions;
            Vector3 speed;
            float speedY = BASE_SPEED_Y - i * SPEED_DAMPENING_FACTOR;
            float positionFactorX = (i * DISTANCE_CROSSING_ASTEROIDS) + 1.2f;
            float positionFactorY = (i * DISTANCE_CROSSING_ASTEROIDS) / (PlayerController.controller.GetSpeed() + 1) * speedY;
            // Alternate positions between low and high in order to cross
            if (i%2 == 0) {
                crossPositions = new Vector2(GameController.GetCameraXMax() + positionFactorX, GameController.GetCameraYMin() - positionFactorY);
                speed = new Vector3(-1, speedY, 0);
            } else {
                crossPositions = new Vector2(GameController.GetCameraXMax() + positionFactorX, GameController.GetCameraYMax() + positionFactorY);
                speed = new Vector3(-1, -speedY, 0);
            }
            GameObject crossingAsteroid = ObjectPool.SharedInstance.SpawnPooledObject(elementType, crossPositions, GameObjectUtil.GenerateRandomRotation());
            IMovingObject crossingMovementScript = crossingAsteroid.GetComponent<IMovingObject>();
            crossingMovementScript.SetSpeed(speed);
        }

        // Final center meteor
        Vector2 position = new Vector2(GameController.GetCameraXMax() + ASTEROID_AMOUNT * 1.5f * DISTANCE_CROSSING_ASTEROIDS, 0);
        observableElement = ObjectPool.SharedInstance.SpawnPooledObject(elementType, position, GameObjectUtil.GenerateRandomRotation());
        IMovingObject movementScript = observableElement.GetComponent<IMovingObject>();
        movementScript.SetSpeed(new Vector3(-2,0,0));
    }
}