﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Make this a scriptable object
public class Constellation : MonoBehaviour
{
    [SerializeField]
    Sprite[] starSprites;

    List<Star> stars = new List<Star>();

    List<Vector2> doomPositions = new List<Vector2>{
        new Vector2(-3, 2.65f), new Vector2(-1.86f, 2.62f), new Vector2(-1.58f, 2.43f), new Vector2(-1.27f, 2.65f), 
        new Vector2(-0.41f, 2.62f), new Vector2(-0.16f, 2.45f), new Vector2(0.11f, 2.65f), new Vector2(0.94f, 2.65f), 
        new Vector2(1.13f, 2.51f), new Vector2(1.33f, 2.68f), new Vector2(1.88f, 2.70f), new Vector2(2.05f, 2.45f), 
        new Vector2(2.33f, 2.62f), new Vector2(3, 2.68f), new Vector2(2.94f, -0.81f), new Vector2(2.72f, -1.01f), 
        new Vector2(2.11f, -0.54f), new Vector2(2.08f, -0.15f), new Vector2(1.86f, -0.34f), new Vector2(1.80f, -0.04f), 
        new Vector2(1.61f, -0.18f), new Vector2(1.13f, 0.23f), new Vector2(0.55f, -0.12f), new Vector2(-0.16f, 0.29f), 
        new Vector2(-0.63f, -0.29f), new Vector2(-1.33f, 0.12f), new Vector2(-2.69f, -0.98f), new Vector2(-2.97f, -0.68f), 
        new Vector2(-2.91f, 0.73f), new Vector2(-1.58f, 1.84f), new Vector2(-0.19f, 0.70f), new Vector2(1.30f, 1.73f), 
        new Vector2(2.88f, 0.62f)
        };

    // Start is called before the first frame update
    void Start() {
        while (doomPositions.Count > 0) {
            FormDoomConstellation(GenerateConstellationStar());
        }
    }

    void FormDoomConstellation(Star star) {
        Vector2 randomPosition = doomPositions[Random.Range(0, doomPositions.Count)];
        doomPositions.Remove(randomPosition);
        star.transform.position = randomPosition;
        stars.Add(star);
    }

    Star GenerateConstellationStar() {
        GameObject newObject = ObjectPool.SharedInstance.SpawnPooledObject(ObjectPool.STAR, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 180)));

        float objectScale = DistributionUtil.GetNormalDistribution(StarGenerator.MIN_STAR_SCALE, StarGenerator.MAX_STAR_SCALE); 
        newObject.transform.localScale = new Vector3(objectScale, objectScale, objectScale);

        newObject.transform.parent = BackgroundStateController.controller.GetStaticBackgroundLayer().transform;
        newObject.AddComponent<LayeredBackgroundObject>();

        // Set sprite randomly
        newObject.GetComponent<SpriteRenderer>().sprite = starSprites[Random.Range(0, starSprites.Length)];

        return newObject.GetComponent<Star>();
    }

    public bool StarInConstellation(Star star) {
        return stars.Contains(star);
    }
}
