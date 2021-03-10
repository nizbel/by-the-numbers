using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementCorridorEvent : ForegroundEvent
{
    ElementsEnum[] elementsAvailable;

    float amount;

    public void SetElementsAvailable(ElementsEnum[] elementsAvailable) {
        this.elementsAvailable = elementsAvailable;
    }

    public void SetAmount(float amount) {
        this.amount = amount;
    }

    protected override void StartEvent() {
        // Spawn elements
        Vector3 spawnPosition = new Vector3(GameController.GetCameraXMax() + 2, GameController.GetCameraYMax() - 0.5f, 0);
        for (int i = 0; i < amount; i++) {
            ObjectPool.SharedInstance.SpawnPooledObject(ChooseElement(), spawnPosition, GameObjectUtil.GenerateRandomRotation());

            // Alternate spawn position
            if (spawnPosition.y > 0) {
                spawnPosition -= spawnPosition.y * Vector3.up * 2;
            } else {
                spawnPosition += new Vector3(2.4f, -2 * spawnPosition.y, 0);
            }
        }

        // Disappear
        Destroy(gameObject);
    }

    ElementsEnum ChooseElement() {
        if (elementsAvailable.Length == 1) {
            return elementsAvailable[0];
        } else {
            return elementsAvailable[Random.Range(0, elementsAvailable.Length)];
        }
    }
}
