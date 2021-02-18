using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// TODO Change to element generator
public class MeteorGenerator : MonoBehaviour
{
    // Constants
    public const float MIN_METEOR_SPEED_LOW_INTENSITY = 1.5f;
    public const float MAX_METEOR_SPEED_LOW_INTENSITY = 3.5f;

    public const float MIN_METEOR_SPEED_HIGH_INTENSITY = 2f;
    public const float MAX_METEOR_SPEED_HIGH_INTENSITY = 4f;

    private const float MIN_SPAWN_COOLDOWN_LOW_INTENSITY = 0.3f;
    private const float MAX_SPAWN_COOLDOWN_LOW_INTENSITY = 0.6f;

    private const float MIN_SPAWN_COOLDOWN_HIGH_INTENSITY = 0.25f;
    private const float MAX_SPAWN_COOLDOWN_HIGH_INTENSITY = 0.5f;

    private const float MIN_SPAWN_LINE_RADIUS = 2.2f;

    // Defines cooldown and average meteor speed
    public enum Intensity {
        Low,
        High
    }

    // The spawn points denote a line from which elements spawn
    // It should be almost perpendicular to the line between the generator and the player
    Vector3 initialSpawnPoint = Vector3.zero;
    Vector3 endSpawnPoint = Vector3.zero;

    // Position that indicates the direction for the elements
    Vector3 attackPoint = Vector3.zero;
    Vector3 attackDirection = Vector3.zero;
    float attackDiretionMagnitude = 0;

    // Generation rate variables
    //float lastSpawn = 0;
    float spawnCoolDown = 0;

    // Intensity variables
    float minSpeed = MIN_METEOR_SPEED_LOW_INTENSITY;
    float maxSpeed = MAX_METEOR_SPEED_LOW_INTENSITY;
    float minCooldown = MIN_SPAWN_COOLDOWN_LOW_INTENSITY;
    float maxCooldown = MAX_SPAWN_COOLDOWN_LOW_INTENSITY;

    // Element to generate
    ElementsEnum elementType = ElementsEnum.ASTEROID;

    // Generation coroutine
    Coroutine generationCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        DefineSpawnCooldown();

        DefineAttackPoint();

        DefineCreationLine();

        generationCoroutine = StartCoroutine(GenerateElements());
        //Debug.Log(attackPoint + "..." + attackDirection + "..." + initialSpawnPoint + "..." + endSpawnPoint);
    }

    void OnDestroy() {
        StopCoroutine(generationCoroutine);
    }

    IEnumerator GenerateElements() {
        while (true) {
            yield return new WaitForSeconds(spawnCoolDown);
        //}
        //if (Time.time - lastSpawn > spawnCoolDown) {
            // Prepare meteor generation
            // Define point of spawn
            Vector3 spawnPoint = Vector3.Lerp(initialSpawnPoint, endSpawnPoint, Random.Range(0f, 1f));

            // Spawn element
            GameObject newMeteor = ObjectPool.SharedInstance.SpawnPooledObject(elementType, spawnPoint, new Quaternion(0, 0, 0, 1));
            newMeteor.transform.localRotation = GameObjectUtil.GenerateRandomRotation();

            float baseSpeed = Random.Range(minSpeed, maxSpeed);
            //baseSpeed = MAX_METEOR_SPEED;
            newMeteor.GetComponent<MovingObject>().Speed = attackDirection / attackDiretionMagnitude * baseSpeed;

            //lastSpawn = Time.time;

            //// Check if spawn cooldown will be changed
            //if (GameController.RollChance(25)) {
                DefineSpawnCooldown();
            //}
        }
    }


    void DefineSpawnCooldown() {
        spawnCoolDown = Random.Range(minCooldown, maxCooldown);
    }

    void DefineAttackPoint() {
        // Always pick the middle of the right most border of the window
        attackPoint = new Vector3(GameController.GetCameraXMax(), 0, 0);
        attackDirection = attackPoint - transform.position;

        attackDiretionMagnitude = attackDirection.magnitude;
    }

    void DefineCreationLine() {
        // Define the radius of the spawn line, the closer to the middle the bigger
        float spawnLineRadius = Mathf.Lerp(MIN_SPAWN_LINE_RADIUS, GameController.GetCameraYMax(), 1 - transform.position.y);
        initialSpawnPoint = new Vector2(-attackDirection.y, attackDirection.x) / attackDiretionMagnitude * spawnLineRadius + new Vector2(transform.position.x, transform.position.y);
        endSpawnPoint = new Vector2(-attackDirection.y, attackDirection.x) / attackDiretionMagnitude * -spawnLineRadius + new Vector2(transform.position.x, transform.position.y);
    }

    public void Enable() {
        enabled = true;
    }

    public void SetIntensity(Intensity intensity) {
        switch (intensity) {
            case Intensity.Low:
                minCooldown = MIN_SPAWN_COOLDOWN_LOW_INTENSITY;
                maxCooldown = MAX_SPAWN_COOLDOWN_LOW_INTENSITY;
                minSpeed = MIN_METEOR_SPEED_HIGH_INTENSITY;
                maxSpeed = MAX_METEOR_SPEED_HIGH_INTENSITY;
                break;

            case Intensity.High:
                minCooldown = MIN_SPAWN_COOLDOWN_HIGH_INTENSITY;
                maxCooldown = MAX_SPAWN_COOLDOWN_HIGH_INTENSITY;
                minSpeed = MIN_METEOR_SPEED_HIGH_INTENSITY;
                maxSpeed = MAX_METEOR_SPEED_HIGH_INTENSITY;
                break;
        }
    }

    public void SetElementType(ElementsEnum elementType) {
        this.elementType = elementType;
    }
}
