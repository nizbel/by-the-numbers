﻿using UnityEngine;

public class SpecialEventDebrisController : ElementSpecialEventController {

    private const int DEBRIS_AMOUNT = 4;

    void Start() {
        // Define element and speeches
        elementType = ObjectPool.DEBRIS;
        observeSpeech = new Speech();
        aboutElementSpeech = new Speech();
}

    protected override void SpawnElement() {
        Vector2 position = new Vector2(GameController.GetCameraXMax() + 1, PlayerController.controller.transform.position.y);
        observableElement = ObjectPool.SharedInstance.SpawnPooledObject(elementType, position, GameObjectUtil.GenerateRandomRotation());

        // Spawn other debris
        for (int i = 0; i < DEBRIS_AMOUNT; i++) {
            position = new Vector2(GameController.GetCameraXMax() + Random.Range(1f, 3f), Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax()));
            ObjectPool.SharedInstance.SpawnPooledObject(elementType, position, GameObjectUtil.GenerateRandomRotation());
        }
    }
}