using UnityEngine;

public class SpecialEventStrayEngineController : ElementSpecialEventController {

    void Start() {
        // Define element and speeches
        elementType = ElementsEnum.STRAY_ENGINE;

        // Increases stage moment's duration to last as much as necessary
        stageMoment.duration = 60;

        ObserveElement();
    }

    protected override void SpawnElement() {
        // Spawn three engines to be activated
        StrayEngine[] horizontalEngines = new StrayEngine[3];
        for (int i = 0; i < horizontalEngines.Length; i++) {
            Vector2 horizontalEnginePosition = new Vector2(GameController.GetCameraXMax() + 1, (i - 1) * Random.Range(GameController.GetCameraYMax() / 2, GameController.GetCameraYMax()));
            horizontalEngines[i] = ObjectPool.SharedInstance.SpawnPooledObject(elementType).GetComponent<StrayEngine>();
            horizontalEngines[i].transform.position = horizontalEnginePosition + GameObjectUtil.GetGameObjectHalfHorizontalSize(horizontalEngines[i].gameObject) * Vector2.left;
            horizontalEngines[i].transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 180f);
            horizontalEngines[i].SetActivatorChance(0);
            horizontalEngines[i].SetRotatingChance(0);
        }

        // Main engine
        float positionX = horizontalEngines[0].transform.position.x
            + GameObjectUtil.GetGameObjectHalfHorizontalSize(horizontalEngines[0].gameObject) + 0.35f;
        Vector2 position = new Vector2(positionX, GameController.GetCameraYMax());
        observableElement = ObjectPool.SharedInstance.SpawnPooledObject(elementType, position, Quaternion.Euler(0f,0f, 270f));
        StrayEngine engineScript = observableElement.GetComponent<StrayEngine>();
        // Guarantees it will activate
        engineScript.SetActivatorChance(100f);
        engineScript.SetShouldShakeOnActivate(false);
        engineScript.SetRotatingChance(0);
        engineScript.SetIsPreActivated(true);

        observableElement.GetComponent<IMovingObject>().SetSpeed((GameController.GetCameraYMax() - GameController.GetCameraYMin()) * Vector3.down * 2);
    }
}