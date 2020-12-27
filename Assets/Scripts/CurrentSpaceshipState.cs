﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentSpaceshipState : MonoBehaviour
{
    [SerializeField]
    Sprite[] sprites;

    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Define current sprite depending on day
        SetCurrentShipSpriteByDay(GameController.controller.GetCurrentDay());

        Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void SetCurrentShipSpriteByDay(int currentDay) {
        // TODO Find a way to get current spaceship damage dynamically

        // TODO Alter these values
        if (currentDay > 80) {
            spriteRenderer.sprite = sprites[3];
            spriteRenderer.material.SetFloat("_CurrentDamageSprite", 0);
            PlayerController.controller.GetBurningAnimationSpriteRenderer().sprite = sprites[3];
        }
        else if (currentDay > 45) {
            spriteRenderer.sprite = sprites[2];
            spriteRenderer.material.SetFloat("_CurrentDamage", (float)(currentDay - 46) / 35);
            PlayerController.controller.GetBurningAnimationSpriteRenderer().sprite = sprites[2];
        }
        else if (currentDay > 15) {
            spriteRenderer.sprite = sprites[1];
            spriteRenderer.material.SetFloat("_CurrentDamage", (float)(currentDay - 16) / 30);
            PlayerController.controller.GetBurningAnimationSpriteRenderer().sprite = sprites[1];
        }
        else {
            // If none of the above, keep the default sprite in the sheet
            spriteRenderer.material.SetFloat("_CurrentDamage", (float)(currentDay - 1) / 15);
        }

        // Set seed for damage material
        Vector2 seed = GameController.GetShipDamageSeed();
        spriteRenderer.material.SetVector("_DamageSeed", new Vector4(seed.x, seed.y, 0, 0));
    }
}