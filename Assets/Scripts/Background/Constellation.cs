using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Make this a scriptable object
public class Constellation : MonoBehaviour
{
    [SerializeField]
    GameObject constellationStarPrefab;

    [SerializeField]
    Sprite[] starSprites;

    List<Star> stars = new List<Star>();

    List<Vector2> doomPositions = new List<Vector2>{
            new Vector2(19,11), new Vector2(60,12), new Vector2(70,19), new Vector2(81,11), new Vector2(112,12),
            new Vector2(121,18), new Vector2(131,11), new Vector2(161,11), new Vector2(168,16), new Vector2(175,10),
            new Vector2(195,9), new Vector2(201,18), new Vector2(211,12), new Vector2(235,10),
            new Vector2(233,136), new Vector2(225,143), new Vector2(203,126), new Vector2(202,112), new Vector2(194,119),
            new Vector2(192,108), new Vector2(185,113), new Vector2(168,98), new Vector2(147,111), new Vector2(121,96),
            new Vector2(104,117), new Vector2(79,102), new Vector2(30,142), new Vector2(20,131), new Vector2(22,80),
            new Vector2(70,40), new Vector2(120,81), new Vector2(174,44), new Vector2(231,84)
        };

    // Start is called before the first frame update
    void Start()
    {
        while (doomPositions.Count > 0) {
            FormDoomConstellation(GenerateConstellationStar().GetComponent<Star>());
        }
    }

    void FormDoomConstellation(Star star) {
        Vector2 randomPosition = doomPositions[Random.Range(0, doomPositions.Count)];
        doomPositions.Remove(randomPosition);
        star.transform.position = new Vector3(-3 + (randomPosition.x - 19f) / 36, (106.5f - randomPosition.y) / 36, 0);
        stars.Add(star);
    }

    GameObject GenerateConstellationStar() {
        float objectScale = DistributionUtil.GetNormalDistribution(StarGenerator.MIN_STAR_SCALE, StarGenerator.MAX_STAR_SCALE);
        GameObject newObject = (GameObject)Instantiate(constellationStarPrefab, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 180)));
        newObject.transform.localScale = new Vector3(objectScale, objectScale, objectScale);

        newObject.transform.parent = BackgroundStateController.controller.GetStaticBackgroundLayer().transform;
        LayeredBackgroundObject layerScript = newObject.AddComponent<LayeredBackgroundObject>();

        // Set sprite randomly
        newObject.GetComponent<SpriteRenderer>().sprite = starSprites[Random.Range(0, starSprites.Length)];

        return newObject;
    }

    public bool StarInConstellation(Star star) {
        return stars.Contains(star);
    }
}
