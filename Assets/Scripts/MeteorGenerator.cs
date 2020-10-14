using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorGenerator : MonoBehaviour
{
    public const float MAX_METEOR_SPEED = 3.5f;
    public const float MIN_METEOR_SPEED = 3.5f;

    private const float MIN_SPAWN_COOLDOWN = 0.2f;
    private const float MAX_SPAWN_COOLDOWN = 0.9f;

    [SerializeField]
    public List<GameObject> meteorList;

    // The spawn points denote a line from which meteors spawn
    // It should be almost perpendicular to the line between the generator and the player
    Vector3 initialSpawnPoint = Vector3.zero;
    Vector3 endSpawnPoint = Vector3.zero;

    // Position that indicates the direction for the meteors
    Vector3 attackPoint = Vector3.zero;
    Vector3 attackDirection = Vector3.zero;
    float attackDiretionMagnitude = 0;

    // Generation rate variables
    float lastSpawn = 0;
    float spawnCoolDown = 0;

    // TODO Remove test
    Vector3 position = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        DefineSpawnCooldown();

        DefineAttackPoint();

        DefineCreationLine();

        // TODO Remove this after test
        position = transform.position;
        //Debug.Log(attackPoint + "..." + attackDirection + "..." + initialSpawnPoint + "..." + endSpawnPoint);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != position) {
            DefineAttackPoint();

            DefineCreationLine();
            position = transform.position;
        }

        if (Time.time - lastSpawn > spawnCoolDown) {
            // Prepare meteor generation
            // Define point of spawn
            Vector3 spawnPoint = Vector3.Lerp(initialSpawnPoint, endSpawnPoint, Random.Range(0f, 1f));

            // Choose among prefabs
            GameObject meteorPrefab = meteorList[Random.Range(0, meteorList.Count)];

            // Spawn element
            GameObject newMeteor = (GameObject)Instantiate(meteorPrefab, spawnPoint, new Quaternion(0, 0, 0, 1));
            // TODO Find somewhere to assign transforms
            //newMeteor.transform.parent = transform;
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

    void FixedUpdate() {
        // TODO Accompanies camera

    }

    void DefineSpawnCooldown() {
        spawnCoolDown = Random.Range(MIN_SPAWN_COOLDOWN, MAX_SPAWN_COOLDOWN);
    }

    void DefineAttackPoint() {
        // Choose randomly
        attackPoint = new Vector3(Random.Range(GameController.GetCameraXMin(), GameController.GetCameraXMax()), 0, 0);
        attackDirection = attackPoint - transform.position;
        attackDiretionMagnitude = attackDirection.magnitude;
        // Check if distance from source to attack isn't too short
        //if (attackDirection.sqrMagnitude < Mathf.Pow(GameController.GetCameraYMax() - GameController.GetCameraYMin(), 2)) {
        //    transform.position = attackPoint - attackDirection * 2;
        //}
    }

    void DefineCreationLine() {
        initialSpawnPoint = new Vector2(-attackDirection.y, attackDirection.x) / attackDiretionMagnitude * 3 + new Vector2(transform.position.x, transform.position.y);
        endSpawnPoint = new Vector2(-attackDirection.y, attackDirection.x) / attackDiretionMagnitude * -3 + new Vector2(transform.position.x, transform.position.y);
    }
}
