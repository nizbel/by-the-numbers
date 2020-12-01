using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NebulaGenerator : MonoBehaviour
{
    private const float MIN_COLOR_TONE = 0.75f;
    private const float MAX_COLOR_TONE = 1.5f;
    private const int MIN_SHADER_SEED = 0;
    private const int MAX_SHADER_SEED = 100;
    private const float MIN_NEBULA_ALPHA = 0.1f;
    private const float MAX_NEBULA_ALPHA = 0.2f;

    public GameObject nebulaPrefab;

    List<GameObject> nebulaSeedList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // Decide how many nebula seeds
        int nebulaAmount = Random.Range(2, 5);

        // Generate nebula seed
        for (int i = 0; i < nebulaAmount; i++) {
            // Instantiate
            GameObject newNebula = GameObject.Instantiate(nebulaPrefab);
            SpriteRenderer nebulaRenderer = newNebula.GetComponent<SpriteRenderer>();
            newNebula.name = "Nebula Seed " + (i + 1);

            // Set random position
            newNebula.transform.position = new Vector3(Random.Range(GameController.GetCameraXMin(), GameController.GetCameraXMax()),
                Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax()), 0);

            // Set random rotation
            newNebula.transform.Rotate(Vector3.forward * Random.Range(0, 360));

            // Set random color
            Color color = new Color(Random.Range(MIN_COLOR_TONE, MAX_COLOR_TONE), Random.Range(MIN_COLOR_TONE, MAX_COLOR_TONE), Random.Range(MIN_COLOR_TONE, MAX_COLOR_TONE), 0);
            nebulaRenderer.color = new Color(1, 1, 1, Random.Range(MIN_NEBULA_ALPHA, MAX_NEBULA_ALPHA));

            // Set seed for shader
            Vector4 seed = new Vector4(Random.Range(MIN_SHADER_SEED, MAX_SHADER_SEED), Random.Range(MIN_SHADER_SEED, MAX_SHADER_SEED), 0, 0);
            nebulaRenderer.material.SetVector("Seed", seed);
            nebulaRenderer.material.SetColor("_Color", color);

            // Expand on the seed
            int followingNebulasAmount = Random.Range(2, 8);

            int angleDirection = Random.Range(0, 360);

            float nebulaRadius = newNebula.transform.localScale.x * nebulaRenderer.sprite.bounds.extents.x;
            Vector3 nextFollowerPosition = GenerateFollowerNebulaPosition(newNebula.transform.position, nebulaRadius, angleDirection);

            // Generate follower nebulas
            for (int j = 0; j < followingNebulasAmount; j++) {
                // Instantiate
                GameObject newFollower = GameObject.Instantiate(nebulaPrefab);
                SpriteRenderer followerRenderer = newFollower.GetComponent<SpriteRenderer>();

                // Set random position
                newFollower.transform.position = nextFollowerPosition;

                // Set random rotation
                newFollower.transform.Rotate(Vector3.forward * Random.Range(0, 360));

                // Define next follower position
                if (j + 1 < followingNebulasAmount) {
                    nebulaRadius = newFollower.transform.localScale.x * followerRenderer.sprite.bounds.extents.x;
                    nextFollowerPosition = GenerateFollowerNebulaPosition(newFollower.transform.position, nebulaRadius, angleDirection);
                }

                // Set random color
                Color followerColor = GenerateColorVariation(color);
                followerRenderer.color = new Color(1, 1, 1, Random.Range(MIN_NEBULA_ALPHA, MAX_NEBULA_ALPHA));

                // Set seed for shader
                Vector4 followerSeed = new Vector4(Random.Range(MIN_SHADER_SEED, MAX_SHADER_SEED), Random.Range(MIN_SHADER_SEED, MAX_SHADER_SEED), 0, 0);
                followerRenderer.material.SetVector("Seed", followerSeed);
                followerRenderer.material.SetColor("_Color", followerColor);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Color GenerateColorVariation(Color color) {
        Color variation = color * Random.Range(0.75f, 1.25f);

        variation.r = Mathf.Clamp(variation.r, MIN_COLOR_TONE, MAX_COLOR_TONE);
        variation.g = Mathf.Clamp(variation.g, MIN_COLOR_TONE, MAX_COLOR_TONE);
        variation.b = Mathf.Clamp(variation.b, MIN_COLOR_TONE, MAX_COLOR_TONE);

        return variation;
    }

    Vector3 GenerateFollowerNebulaPosition(Vector3 basePosition, float radius, int angleDirection) {
        float randomAngle = (angleDirection + Random.Range(-15, 16)) * Mathf.Deg2Rad;
        Vector3 nextPosition = basePosition + new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0) * radius;

        if (nextPosition.x > GameController.GetCameraXMax()) {
            nextPosition.x -= (GameController.GetCameraXMax() - GameController.GetCameraXMin());
            Debug.Log("Added screen X");
        } else if (nextPosition.x < GameController.GetCameraXMin()) {
            nextPosition.x += (GameController.GetCameraXMax() - GameController.GetCameraXMin());
            Debug.Log("Subtracted screen X");
        }

        if (nextPosition.y > GameController.GetCameraYMax()) {
            nextPosition.y -= (GameController.GetCameraYMax() - GameController.GetCameraYMin());
            Debug.Log("Added screen Y");
        } else if (nextPosition.y < GameController.GetCameraYMin()) {
            nextPosition.y += (GameController.GetCameraYMax() - GameController.GetCameraYMin());
            Debug.Log("Subtracted screen Y");
        }

        Debug.Log(basePosition + "..." + angleDirection);

        return nextPosition;
    }
}
