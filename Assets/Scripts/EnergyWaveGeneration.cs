using System.Collections;
using UnityEngine;

// TODO Change name to be any element wave generator
public class EnergyWaveGeneration : MonoBehaviour
{
    // Intervals
    private const float SHORT_GENERATION_INTERVAL = 0.15f;
    private const float DEFAULT_GENERATION_INTERVAL = 0.2f;
    private const float LONG_GENERATION_INTERVAL = 0.3f;

    public enum GenerationIntervalEnum {
        Short,
        Default,
        Long
    }

    public enum WaveTypeEnum {
        RandomElements,
        SequentialElements,
        SequentialVerticalDirection,
        SequentialHorizontalDirection
    }

    // Wave type
    WaveTypeEnum type = WaveTypeEnum.RandomElements;


    ElementsEnum[] availableElements;

    // Used to keep track of which element to spawn
    int currentElementIndex = 0;

    float generationInterval = DEFAULT_GENERATION_INTERVAL;

    float currentPositionY;

    // Offset for wave vertical position
    float centerPositionY = 0;

    Coroutine generation = null;

    // Size in world units of the sine radius
    float amplitude = 1;

    float startingAngle = 0;

    float currentAngle;

    float frequency = 1;

    // Duration of the generator
    float duration;

    // Starting point for the frequency calculation
    float startTime;

    // Start is called before the first frame update
    void Start()
    {
        // Define star time
        startTime = Time.time;

        // If available elements not set, pick current stage moments
        if (availableElements == null) {
            availableElements = StageController.controller.GetCurrentMomentAvailableElements();
        }

        generation = StartCoroutine(GenerateWave());
    }

    void FixedUpdate() {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0) {
            StopCoroutine(generation);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    IEnumerator GenerateWave()
    {
        while (true) {
            Vector3 nextPosition = new Vector3(GameController.GetCameraXMax() + 2, 0, 0);
            ElementsEnum elementType = DefineElementType();

            // Define Y position
            DefinePositionY();

            // Add next element in position
            GameObject nextElement = ObjectPool.SharedInstance.SpawnPooledObject(elementType);
            nextElement.transform.position = nextPosition + Vector3.up * currentPositionY;

            yield return new WaitForSeconds(generationInterval);
        }
    }

    // Define next element
    private ElementsEnum DefineElementType() {
        switch (type) {
            case WaveTypeEnum.RandomElements:
                return availableElements[Random.Range(0, availableElements.Length)];

            case WaveTypeEnum.SequentialElements:
                // Advance to the next element
                currentElementIndex = (currentElementIndex + 1) % availableElements.Length;
                return availableElements[currentElementIndex];

            case WaveTypeEnum.SequentialVerticalDirection:
            case WaveTypeEnum.SequentialHorizontalDirection:
                return availableElements[currentElementIndex];

            default:
                // Fallback is random elements
                return availableElements[Random.Range(0, availableElements.Length)];
        }
    }

    void DefinePositionY() {
        float newAngle = ((Time.time - startTime) * 2 * Mathf.PI * frequency + startingAngle * Mathf.Deg2Rad) % (2 * Mathf.PI);
        // Check if angle change should impact current element choice
        if (type == WaveTypeEnum.SequentialVerticalDirection) {
            // If vertical direction changed, advance to the next element
            if (AngleCrossedThresholdRad(currentAngle, newAngle, Mathf.PI/2) || (AngleCrossedThresholdRad(currentAngle, newAngle, 3 * Mathf.PI / 2))) {
                currentElementIndex = (currentElementIndex + 1) % availableElements.Length;
            }
        }
        else if (type == WaveTypeEnum.SequentialHorizontalDirection) {
            // If horizontal direction changed, advance to the next element
            if (AngleCrossedThresholdRad(currentAngle, newAngle, Mathf.PI) || (AngleCrossedThresholdRad(currentAngle, newAngle, 2 * Mathf.PI))) {
                currentElementIndex = (currentElementIndex + 1) % availableElements.Length;
            }

        }
        currentAngle = newAngle;
        currentPositionY = Mathf.Sin(currentAngle) * amplitude + centerPositionY;
    }

    // Checks if angle change crossed a threshold, all values in radians
    bool AngleCrossedThresholdRad(float oldValue, float newValue, float threshold) {
        return oldValue <= threshold && newValue > threshold;
    }

    public void SetAvailableElements(ElementsEnum[] availableElements) {
        this.availableElements = availableElements;
    }

    public void SetDuration(float duration) {
        this.duration = duration;
    }

    public void SetAmplitude(float amplitude) {
        this.amplitude = amplitude;
    }

    public void SetFrequency(float frequency) {
        this.frequency = frequency;
    }

    public void SetStartingAngle(float startingAngle) {
        this.startingAngle = startingAngle;
    }

    public void SetCenterPositionY(float centerPositionY) {
        this.centerPositionY = centerPositionY;
    }

    public void SetGenerationInterval(float generationInterval) {
        this.generationInterval = generationInterval;
    }

    public void SetType(WaveTypeEnum type) {
        this.type = type;
    }
}
