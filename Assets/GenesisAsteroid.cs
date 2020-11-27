using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenesisAsteroid : MonoBehaviour
{
    private const float DEFAULT_CYCLE = 1.5f;
    private const float IDLE_DURATION = DEFAULT_CYCLE * 0.1f;
    private const float DEVELOPING_DURATION = DEFAULT_CYCLE * 0.4f;
    private const float SEPARATING_DURATION = DEFAULT_CYCLE * 0.4f;
    private const float LAUNCHING_DURATION = DEFAULT_CYCLE * 0.1f;

    private const int IDLE = 1;
    private const int DEVELOPING = 2;
    private const int SEPARATING = 3;
    private const int LAUNCHING = 4;

    // TODO Remove serialization
    [SerializeField]
    float currentPointInCycle;

    // TODO Remove serialization
    [SerializeField]
    int state;

    // Generating points that vary for each sprite object
    [SerializeField]
    Vector3 generatingPointPositive;
    [SerializeField]
    Vector3 generatingPointNegative;

    // TODO Remove once we get to use object generators
    [SerializeField]
    GameObject positiveEnergyPrefab;
    [SerializeField]
    GameObject negativeEnergyPrefab;

    // Materials for the lifecycle
    [SerializeField]
    Material idleMaterial = null;
    [SerializeField]
    Material developmentMaterial = null;
    [SerializeField]
    Material separatingMaterial = null;
    Vector2 textureGenerationOffset;

    // Particle systems for the launching animation
    [SerializeField]
    ParticleSystem positiveFocus;
    [SerializeField]
    ParticleSystem negativeFocus;

    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Starts anytime in the cycle, except for its ending
        currentPointInCycle = Random.Range(0, DEFAULT_CYCLE*0.9f);
        DefineState();

        // Scale generating points
        generatingPointPositive *= transform.localScale.x;
        generatingPointNegative *= transform.localScale.x;

        // Scale particle systems radius
        ParticleSystem.ShapeModule positiveShape = positiveFocus.shape;
        positiveShape.radius *= transform.localScale.x;
        ParticleSystem.ShapeModule negativeShape = negativeFocus.shape;
        negativeShape.radius *= transform.localScale.x;

        // Define random animation offset for generation
        textureGenerationOffset = new Vector2(Random.Range(0,1f), Random.Range(0,1f));
    }

    // Update is called once per frame
    void Update()
    {
        currentPointInCycle += Time.deltaTime;
        switch (state) {
            case IDLE:
                if (currentPointInCycle >= IDLE_DURATION) {
                    DevelopParticles();
                }
                break;

            // Develop particles
            case DEVELOPING:
                // TODO Animate shader
                float fillAmount = Mathf.Lerp(0, 1, currentPointInCycle / DEVELOPING_DURATION);
                spriteRenderer.material.SetFloat("_FillAmount", fillAmount);

                if (currentPointInCycle >= DEVELOPING_DURATION) {
                    SeparateParticles();
                }
                break;

            // Separate
            case SEPARATING:
                // TODO Animate shader
                float separatingAmount = Mathf.Lerp(0, 1, currentPointInCycle / DEVELOPING_DURATION);
                spriteRenderer.material.SetFloat("_FillAmount", separatingAmount);

                if (currentPointInCycle >= SEPARATING_DURATION) {
                    GenerateEnergies();
                }
                break;

            // Launch particles
            case LAUNCHING:
                // TODO Animate launching
                if (currentPointInCycle >= LAUNCHING_DURATION) {

                    // Finish launching
                    GameObject positiveEnergy = GameObject.Instantiate(positiveEnergyPrefab);
                    positiveEnergy.transform.position = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * generatingPointPositive + transform.position;
                    Vector3 distance = positiveEnergy.transform.position - transform.position;
                    // Change its random sizing script
                    RandomSize randomSizeScript = positiveEnergy.GetComponent<RandomSize>();
                    randomSizeScript.SetStartVarying(true);
                    randomSizeScript.SetScalingSpeed(7.5f);

                    positiveEnergy.GetComponent<Rigidbody2D>().AddForce(distance * Mathf.Pow(distance.sqrMagnitude + 1, 2));

                    GameObject negativeEnergy = GameObject.Instantiate(negativeEnergyPrefab);
                    negativeEnergy.transform.position = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * generatingPointNegative + transform.position;
                    distance = negativeEnergy.transform.position - transform.position;
                    // Change its random sizing script
                    randomSizeScript = negativeEnergy.GetComponent<RandomSize>();
                    randomSizeScript.SetStartVarying(true);
                    randomSizeScript.SetScalingSpeed(7.5f);

                    negativeEnergy.GetComponent<Rigidbody2D>().AddForce(distance * Mathf.Pow(distance.sqrMagnitude + 1, 2));

                    //Debug.Break();
                    spriteRenderer.material = idleMaterial;


                    positiveFocus.Stop();
                    negativeFocus.Stop();

                    SetState(IDLE);
                }
                break;
        }
    }

    void DevelopParticles() {
        spriteRenderer.material = developmentMaterial;
        spriteRenderer.material.SetFloat("_FillAmount", 0);
        spriteRenderer.material.SetVector("_FillTexture_ST", new Vector4(1, 1, textureGenerationOffset.x, textureGenerationOffset.y));

        SetState(DEVELOPING);
    }

    void SeparateParticles() {
        spriteRenderer.material = separatingMaterial;
        spriteRenderer.material.SetFloat("_FillAmount", 0);
        spriteRenderer.material.SetVector("_FillTexture_ST", new Vector4(1, 1, textureGenerationOffset.x, textureGenerationOffset.y));

        SetState(SEPARATING);
    }

    void GenerateEnergies() {
        positiveFocus.Play();
        negativeFocus.Play();

        SetState(LAUNCHING);
    }

    // Define current state depending on what point it is, never LAUNCHING
    void DefineState() {
        if (currentPointInCycle < IDLE_DURATION) {
            state = IDLE;
            spriteRenderer.material = idleMaterial;

        } else if (currentPointInCycle < IDLE_DURATION + DEVELOPING_DURATION) {
            state = DEVELOPING;
            currentPointInCycle -= IDLE_DURATION;
            spriteRenderer.material = developmentMaterial;
            spriteRenderer.material.SetFloat("_FillAmount", currentPointInCycle / DEVELOPING_DURATION);

        } else {
            state = SEPARATING;
            currentPointInCycle -= (IDLE_DURATION + DEVELOPING_DURATION);
            spriteRenderer.material = separatingMaterial;
            spriteRenderer.material.SetFloat("_FillAmount", currentPointInCycle / SEPARATING_DURATION);

        }
    }

    void SetState(int state) {
        this.state = state;
        currentPointInCycle = 0;
    }
}
