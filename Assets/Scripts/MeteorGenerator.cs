using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorGenerator : MonoBehaviour
{
    // Constants
    public const float MAX_METEOR_SPEED = 5.5f;
    public const float MIN_METEOR_SPEED = 8.5f;

    private const float MIN_SPAWN_COOLDOWN = 0.2f;
    private const float MAX_SPAWN_COOLDOWN = 0.9f;

    private const float SPAWN_LINE_RADIUS = 3.5f;

    [SerializeField]
    public List<GameObject> meteorList;

    // The spawn points denote a line from which meteors spawn
    // It should be almost perpendicular to the line between the generator and the player
    Vector3 initialSpawnPoint = Vector3.zero;
    Vector3 endSpawnPoint = Vector3.zero;

    // Keep distance to camera during camera movement
    //Vector3 cameraDisplacement = Vector3.zero;

    // Position that indicates the direction for the meteors
    Vector3 attackPoint = Vector3.zero;
    Vector3 attackDirection = Vector3.zero;
    float attackDiretionMagnitude = 0;

    // Generation rate variables
    float lastSpawn = 0;
    float spawnCoolDown = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Define camera displacement
        //cameraDisplacement = transform.position - Camera.main.transform.position;

        if (transform.position.x < GameController.GetCameraXMax()) {
            transform.position += new Vector3(SPAWN_LINE_RADIUS, 0, 0);
        }

        DefineSpawnCooldown();

        DefineAttackPoint();

        DefineCreationLine();

        //Debug.Log(attackPoint + "..." + attackDirection + "..." + initialSpawnPoint + "..." + endSpawnPoint);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastSpawn > spawnCoolDown) {
            // Prepare meteor generation
            // Define point of spawn
            Vector3 spawnPoint = Vector3.Lerp(initialSpawnPoint, endSpawnPoint, Random.Range(0f, 1f));

            // Choose among prefabs
            GameObject meteorPrefab = meteorList[Random.Range(0, meteorList.Count)];

            // Spawn element
            GameObject newMeteor = (GameObject)Instantiate(meteorPrefab, spawnPoint, new Quaternion(0, 0, 0, 1));
            newMeteor.transform.localRotation = GameObjectUtil.GenerateRandomRotation();

            float baseSpeed = Random.Range(MIN_METEOR_SPEED, MAX_METEOR_SPEED);
            newMeteor.GetComponent<MovingObject>().Speed = attackDirection / attackDiretionMagnitude * baseSpeed;

            lastSpawn = Time.time;

            // Check if spawn cooldown will be changed
            if (GameController.RollChance(25)) {
                DefineSpawnCooldown();
            }
        }
    }

    //void FixedUpdate() {
    //    // TODO Accompanies camera
    //    // Get initial position to catch X axis displacement
    //    float initialPositionX = transform.position.x;

    //    // Change spawning positions
    //    transform.position = Camera.main.transform.position + cameraDisplacement;

    //    // Update all vectors
    //    Vector3 generatorXDisplacement = new Vector3(transform.position.x - initialPositionX, 0, 0);
    //    initialSpawnPoint = initialSpawnPoint + generatorXDisplacement;
    //    endSpawnPoint = endSpawnPoint + generatorXDisplacement;
    //    // TODO Decide if this should be updated
    //    //attackPoint = attackPoint + generatorXDisplacement;
    //}

    void DefineSpawnCooldown() {
        spawnCoolDown = Random.Range(MIN_SPAWN_COOLDOWN, MAX_SPAWN_COOLDOWN);
    }

    void DefineAttackPoint() {
        // Choose randomly
        float halfScreen = (GameController.GetCameraXMax() - GameController.GetCameraXMin())/2;
        attackPoint = new Vector3(Random.Range(GameController.GetCameraXMin() + halfScreen, GameController.GetCameraXMax()), 0, 0);
        attackDirection = attackPoint - transform.position;
        // Adjust X axis in order to have the meteors a bit more to the right
        if (attackDirection.x < PlayerController.controller.GetSpeed()-3) {
            attackDirection += new Vector3(4, 0, 0);
        }
        attackDiretionMagnitude = attackDirection.magnitude;
        // Check if distance from source to attack isn't too short
        //if (attackDirection.sqrMagnitude < Mathf.Pow(GameController.GetCameraYMax() - GameController.GetCameraYMin(), 2)) {
        //    transform.position = attackPoint - attackDirection * 2;
        //}
    }

    void DefineCreationLine() {
        initialSpawnPoint = new Vector2(-attackDirection.y, attackDirection.x) / attackDiretionMagnitude * SPAWN_LINE_RADIUS + new Vector2(transform.position.x, transform.position.y);
        endSpawnPoint = new Vector2(-attackDirection.y, attackDirection.x) / attackDiretionMagnitude * -SPAWN_LINE_RADIUS + new Vector2(transform.position.x, transform.position.y);

        //GameObject meteorPrefab = meteorList[Random.Range(0, meteorList.Count)];
        //GameObject newMeteor1 = (GameObject)Instantiate(meteorPrefab, initialSpawnPoint, new Quaternion(0, 0, 0, 1));
        //GameObject newMeteor2 = (GameObject)Instantiate(meteorPrefab, endSpawnPoint, new Quaternion(0, 0, 0, 1));

        //Debug.Break();
    }
}
