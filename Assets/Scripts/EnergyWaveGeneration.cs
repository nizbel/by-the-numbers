using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Change name to be any element wave generator
public class EnergyWaveGeneration : MonoBehaviour
{
    private const float DEFAULT_GENERATION_INTERVAL = 0.2f;

    ElementsEnum[] availableElements;

    float generationInterval = DEFAULT_GENERATION_INTERVAL;

    float currentPositionY;

    // Offset for wave vertical position
    float centerPositionY = 0;

    Coroutine generation = null;

    // Size in world units of the sine radius
    float amplitude = 1;

    float startingAngle = 0;

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

            // Add two energies
            //GameObject positiveEnergy = ObjectPool.SharedInstance.SpawnPooledObject(ElementsEnum.POSITIVE_ENERGY);
            //positiveEnergy.transform.position = nextPosition + Vector3.up * currentPositionY * direction;
            //GameObject negativeEnergy = ObjectPool.SharedInstance.SpawnPooledObject(ElementsEnum.NEGATIVE_ENERGY);
            //negativeEnergy.transform.position = nextPosition + Vector3.up * -currentPositionY * direction;

            // Define next element
            ElementsEnum elementType = availableElements[Random.Range(0, availableElements.Length)];

            // Add next element in position
            GameObject nextElement = ObjectPool.SharedInstance.SpawnPooledObject(elementType);
            // Define Y position
            DefinePositionY();
            nextElement.transform.position = nextPosition + Vector3.up * currentPositionY;

            yield return new WaitForSeconds(generationInterval);
        }
    }

    void DefinePositionY() {
        float currentFrequency = (Time.time - startTime) * 2 * Mathf.PI * frequency + startingAngle * Mathf.Deg2Rad;
        Debug.Log(currentFrequency + "..." + Mathf.Sin(currentFrequency));
        currentPositionY = Mathf.Sin(currentFrequency) * amplitude + centerPositionY;
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
}
