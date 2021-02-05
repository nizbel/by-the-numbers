﻿using UnityEngine;

public class Nebula : MonoBehaviour
{
    private const float ALTERATION_CHANCE = 25;
    private const float MAX_CHANGE_SPEED = 0.25f;

    float speedX;

    float speedY;

    Material material;

    // Start is called before the first frame update
    void Start()
    {
        if (GameController.RollChance(ALTERATION_CHANCE)) {
            material = GetComponent<SpriteRenderer>().material;
            speedX = Random.Range(-MAX_CHANGE_SPEED, MAX_CHANGE_SPEED);
            speedY = Random.Range(-MAX_CHANGE_SPEED, MAX_CHANGE_SPEED);
        } else {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector4 seed = material.GetVector("Seed");
        seed += new Vector4(speedX, speedY, 0, 0) * Time.deltaTime;
        material.SetVector("Seed", seed);
    }
}
