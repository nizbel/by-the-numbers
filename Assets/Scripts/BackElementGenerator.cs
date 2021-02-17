using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackElementGenerator : MonoBehaviour {

    // Amounts
    private const int LOW_AMOUNT = 1;
    private const int MIN_MEDIUM_AMOUNT = 2;
    private const int MAX_MEDIUM_AMOUNT = 4;
    private const int MIN_HIGH_AMOUNT = 5;
    private const int MAX_HIGH_AMOUNT = 7;

    // Duration
    private const float MIN_DURATION = 1.5f;
    private const float MAX_DURATION = 2.5f;
    private const float TIME_BETWEEN_ELEMENTS = 0.6f;

    public enum AmountEnum {
        Low,
        Medium,
        High
    }

    ElementsEnum[] elementsAvailable;

    int elementsAmount;

    int generatedElements = 0;

    // Vertical position for generation
    float generatingPositionY;
    float targetPositionY;

    float duration;

    List<BackElement> elements = new List<BackElement>();

    class BackElement {
        public float duration;
        public float speedX;
        public Rigidbody2D body;
        public DestructibleObject destructibleScript;

        public BackElement(Rigidbody2D body, DestructibleObject destructibleScript, float duration) {
            this.body = body;
            this.destructibleScript = destructibleScript;
            this.duration = duration;
        }

    }

    // Start is called before the first frame update
    void Start() {
        duration = Random.Range(MIN_DURATION, MAX_DURATION);

        // Define starting position
        DefinePositions();

        StartCoroutine(GenerateElements());
    }

    // Update is called once per frame
    void Update() {
        float playerSpeed = PlayerController.controller.GetSpeed();
        float speedY = (targetPositionY - generatingPositionY)/duration;
        for (int i = elements.Count-1; i >= 0; i--) {
            BackElement element = elements[i];
            element.speedX = Mathf.Lerp(playerSpeed * 3f, - playerSpeed, 1 - (element.duration / duration));
            element.body.transform.position += new Vector3(element.speedX, speedY, 0) * Time.deltaTime;
            //element.body.velocity = new Vector2(element.speedX, speedY);
            element.duration -= Time.deltaTime;

            if (element.duration <= 0) {
                element.destructibleScript.SetIsDestructibleNow(true);
                elements.RemoveAt(i);
            }
        }

        // Destroy itself on end
        if (elements.Count == 0 && elementsAmount == generatedElements) {
            Destroy(gameObject);
        }
    }

    IEnumerator GenerateElements() {
        do {
            // Define element type
            ElementsEnum elementType = elementsAvailable[Random.Range(0, elementsAvailable.Length)];

            // Generate element
            DestructibleObject newElement = ObjectPool.SharedInstance.SpawnPooledObject(elementType).GetComponent<DestructibleObject>();
            newElement.transform.position = new Vector3(GameController.GetCameraXMin() - GameObjectUtil.GetBiggestSideOfSprite(newElement.GetSpriteRenderer().sprite) - 0.5f, 
                generatingPositionY, 0);
            newElement.transform.rotation = GameObjectUtil.GenerateRandomRotation();
            newElement.SetIsDestructibleNow(false);

            // Add back element to list
            elements.Add(new BackElement(newElement.GetComponent<Rigidbody2D>(), newElement, duration));
            generatedElements++;

            // Wait
            yield return new WaitForSeconds(TIME_BETWEEN_ELEMENTS);
        }
        while (generatedElements < elementsAmount);
    }

    void DefinePositions() {
        float playerPositionY = PlayerController.controller.transform.position.y;
        if (playerPositionY > 0) {
            generatingPositionY = Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMin() + playerPositionY);
            targetPositionY = Random.Range(0, GameController.GetCameraYMax());
        }
        else if (playerPositionY < 0) {
            generatingPositionY = Random.Range(GameController.GetCameraYMax() + playerPositionY, GameController.GetCameraYMax());
            targetPositionY = Random.Range(GameController.GetCameraYMin(), 0);
        }
        else if (playerPositionY == 0) {
            if (GameController.RollChance(50)) {
                generatingPositionY = GameController.GetCameraYMax();
                targetPositionY = Random.Range(GameController.GetCameraYMin(), 0);
            } else {
                generatingPositionY = GameController.GetCameraYMin();
                targetPositionY = Random.Range(0, GameController.GetCameraYMax());
            }
        }
    }

    public void SetElementsAvailable(ElementsEnum[] elementsAvailable) {
        this.elementsAvailable = elementsAvailable;
    }

    public void SetAmount(AmountEnum amount) {
        switch (amount) {
            case AmountEnum.Low:
                elementsAmount = LOW_AMOUNT;
                break;
            case AmountEnum.Medium:
                elementsAmount = Random.Range(MIN_MEDIUM_AMOUNT, MAX_MEDIUM_AMOUNT);
                break;
            case AmountEnum.High:
                elementsAmount = Random.Range(MIN_HIGH_AMOUNT, MAX_HIGH_AMOUNT);
                break;
        }
    }
}
