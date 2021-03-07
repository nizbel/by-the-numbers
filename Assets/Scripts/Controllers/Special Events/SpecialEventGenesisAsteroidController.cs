using UnityEngine;

public class SpecialEventGenesisAsteroidController : ElementSpecialEventController {

    void Start() {
        // Define element and speeches
        elementType = ElementsEnum.GENESIS_ASTEROID;

        // Increases stage moment's duration to last as much as necessary
        stageMoment.duration = 60;

        ObserveElement();
    }

    protected override void SpawnElement() {
        // Spawn genesis asteroid in the distant foreground
        //observableElement = ObjectPool.SharedInstance.SpawnPooledObject(ElementsEnum.DF_GENESIS_ASTEROID);
        observableElement = BackgroundStateController.controller.GetDistantForegroundGenerator()
            .GenerateSpecificDistantForegroundElement(ElementsEnum.DF_GENESIS_ASTEROID, Vector3.zero, BackgroundLayerEnum.SlowestDistantForegroundLayer);

        observableElement.transform.position = new Vector3(GameController.GetCameraXMax() + 2, Random.Range(-0.5f, 0.5f), 0);
        observableElement.GetComponent<IMovingObject>().SetSpeed(Vector3.right * PlayerController.controller.GetSpeed()/2);
    }
}