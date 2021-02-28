using UnityEngine;
using System.Collections;

public class SpecialEventControllerDay29 : MonoBehaviour {
    private const float DEFAULT_SPAWN_OFFSET_X = 2;

    private const int SONG_STATE_1 = 1;
    private const int SONG_STATE_2 = 2;
    private const int SONG_STATE_3 = 3;
    private const int SONG_STATE_4 = 4;
    private const int END_STATE = 5;

    float duration;

    float defaultSpawnPositionX;

    // Period an element takes between spawning and hitting the ship
    float periodBetweenSpawnAndShip;

    [SerializeField]
    float songStartTime;

    float bpm = 90;

    // Timestamps for song state transitions
    float[] transitions = new float[] { 18.8f, 42.2f, 53f };

    int state = SONG_STATE_1;

    // Count how many beats have happened since last change
    int beatCount = 0;
    // Controls current beat state
    int beatState = 1;

    Coroutine generation;

    // Use this for initialization
    void Start() {
        // Define default spawn position
        defaultSpawnPositionX = GameController.GetCameraXMax() + DEFAULT_SPAWN_OFFSET_X;

        // Time between spawn and ship
        periodBetweenSpawnAndShip = (defaultSpawnPositionX - PlayerController.controller.transform.position.x) / PlayerController.controller.GetSpeed();

        // Fill duration
        duration = StageController.controller.GetCurrentMomentDuration();

        generation = StartCoroutine(GenerateEnergiesByBPM());
    }

    private void FixedUpdate() {
        duration -= Time.fixedDeltaTime;
        // Song state progression
        if (state < SONG_STATE_4) {
            if (transitions[state-1] - Time.timeSinceLevelLoad <= 0) {
                state++;
                beatCount = 0;
            }
        }
        if (duration <= 0) {
            StopCoroutine(generation);
            Destroy(gameObject);
        }
    }

    void SpawnEnergies() {
        ElementsEnum energyType = PlayerController.controller.GetValue() >= 0 ? ElementsEnum.NEGATIVE_ENERGY : ElementsEnum.POSITIVE_ENERGY;

        Vector3 position = DefinePosition();
        if (position != Vector3.zero) {
            GameObject newEnergy = ObjectPool.SharedInstance.SpawnPooledObject(energyType,
                position, Quaternion.identity);

            if (state == END_STATE) {
                newEnergy.GetComponent<Energy>().AddDisappearListener(GenerateExplosionEffect);
            }
        }
    }

    IEnumerator GenerateEnergiesByBPM() {
        yield return new WaitForSeconds(songStartTime - periodBetweenSpawnAndShip - Time.timeSinceLevelLoad);
        while (true) {
            // Using delta time to avoid accumulating frame processing delays into bpm
            SpawnEnergies();
            yield return new WaitForSeconds(60 / bpm - Time.deltaTime);
        }
    }

    Vector3 DefinePosition() {
        switch (state) {
            case SONG_STATE_1:
                return new Vector3(defaultSpawnPositionX, Random.Range(GameController.GetCameraYMin() + 1, GameController.GetCameraYMax() - 1), 0);
            case SONG_STATE_2:
                beatCount++;
                if (beatCount < 4) {
                    return new Vector3(defaultSpawnPositionX, Random.Range(GameController.GetCameraYMin() + 1, -0.5f), 0);
                } else {
                    beatCount = 0;
                    return new Vector3(defaultSpawnPositionX, 1f, 0);
                }
            case SONG_STATE_3:
                beatCount++;
                if (beatState == 1) {
                    beatState++;
                    beatCount = 0;
                    return new Vector3(defaultSpawnPositionX, 2f, 0);
                } else {
                    if (beatState == 5) {
                        return Vector3.zero;
                    } else if (beatCount % 3 == 0) {
                        return new Vector3(defaultSpawnPositionX, 1f, 0);
                    } else if (beatCount % 4 == 0) {
                        beatState++;
                        beatCount = 0;
                        return new Vector3(defaultSpawnPositionX, 0f, 0);
                    }
                    else {
                        return Vector3.zero;
                    }
                }
            case SONG_STATE_4:
                state++;

                return new Vector3(defaultSpawnPositionX, 0f, 0);
            default:
                return Vector3.zero;
        }
    }

    void GenerateExplosionEffect() {
        Vector3 position = Vector3.zero;
        for (int i = 0; i < 30; i++) {
            ElementsEnum energyType = i % 2 == 0 ? ElementsEnum.POSITIVE_ENERGY : ElementsEnum.NEGATIVE_ENERGY;
            if (i % 2 == 0) {
                position = new Vector3((i/2)*0.6f - 1, Random.Range(-2, 2f), 0);
            }

            GameObject newEnergy = ObjectPool.SharedInstance.SpawnPooledObject(energyType,
                   position, Quaternion.identity);
        }
    }
}