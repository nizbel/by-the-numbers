using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyWaveGeneration : MonoBehaviour
{
    private const float DEFAULT_GENERATION_INTERVAL = 0.2f;

    ElementsEnum[] availableElements;

    float nextGeneration = DEFAULT_GENERATION_INTERVAL;

    float currentPositionY;

    Coroutine generation = null;

    float amplitude;

    float frequency;

    // Duration of the generator
    float duration;

    // Start is called before the first frame update
    void Start()
    {
        DefinePositionY();
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
            yield return new WaitForSeconds(nextGeneration);

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

        }
    }

    void DefinePositionY() {
        currentPositionY = (Mathf.Sin(Time.time * frequency)) * amplitude;
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
}
