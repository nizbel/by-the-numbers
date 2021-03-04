using UnityEngine;

public class StrayEnginesActivationEvent : ForegroundEvent {
    private const int MIN_SHORT_AMOUNT = 1;
    private const int MAX_SHORT_AMOUNT = 2;
    private const int MIN_LONG_AMOUNT = 3;
    private const int MAX_LONG_AMOUNT = 4;

    public enum AmountEnum {
		Low,
		High
	}

    AmountEnum amount;

    public void SetAmount(AmountEnum amount) {
        this.amount = amount;
    }

    protected override void StartEvent() {
        // Spawn three engines to be activated
        StrayEngine[] horizontalEngines = new StrayEngine[DefineAmount()];
        for (int i = 0; i < horizontalEngines.Length; i++) {
            Vector2 horizontalEnginePosition = new Vector2(GameController.GetCameraXMax() + 1, (i - 1) * Random.Range(GameController.GetCameraYMax() / 2, GameController.GetCameraYMax()));
            horizontalEngines[i] = ObjectPool.SharedInstance.SpawnPooledObject(ElementsEnum.STRAY_ENGINE).GetComponent<StrayEngine>();
            horizontalEngines[i].transform.position = horizontalEnginePosition + GameObjectUtil.GetGameObjectHalfHorizontalSize(horizontalEngines[i].gameObject) * Vector2.left;
            horizontalEngines[i].transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 180f);
            horizontalEngines[i].SetActivatorChance(0);
            horizontalEngines[i].SetRotatingChance(0);
        }

        // Main engine
        float positionX = horizontalEngines[0].transform.position.x
            + GameObjectUtil.GetGameObjectHalfHorizontalSize(horizontalEngines[0].gameObject) + 0.35f;
        Vector2 position = new Vector2(positionX, GameController.GetCameraYMax());
        StrayEngine activationEngine = ObjectPool.SharedInstance.SpawnPooledObject(ElementsEnum.STRAY_ENGINE, position, Quaternion.Euler(0f, 0f, 270f)).GetComponent<StrayEngine>();
        // Guarantees it will activate
        activationEngine.SetActivatorChance(100f);
        activationEngine.SetShouldShakeOnActivate(false);
        activationEngine.SetRotatingChance(0);
        activationEngine.SetIsPreActivated(true);

        activationEngine.GetComponent<IMovingObject>().SetSpeed((GameController.GetCameraYMax() - GameController.GetCameraYMin()) * Vector3.down * 2);

        // Disappear
        Destroy(gameObject);
    }


    private int DefineAmount() {
        switch (amount) {
            case AmountEnum.Low:
                return Random.Range(MIN_SHORT_AMOUNT, MAX_SHORT_AMOUNT);

            case AmountEnum.High:
                return Random.Range(MIN_LONG_AMOUNT, MAX_LONG_AMOUNT);

            default:
                return 0;
        }
    }
}
