using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenesisAsteroid : DestructibleObject
{
    private const float DEFAULT_CYCLE = 1.5f;
    private const float IDLE_DURATION = DEFAULT_CYCLE * 0.1f;
    private const float DEVELOPING_DURATION = DEFAULT_CYCLE * 0.4f;
    private const float SEPARATING_DURATION = DEFAULT_CYCLE * 0.4f;
    private const float LAUNCHING_DURATION = DEFAULT_CYCLE * 0.1f;

    // States
    private const int IDLE = 1;
    private const int DEVELOPING = 2;
    private const int SEPARATING = 3;
    private const int LAUNCHING = 4;

    // Speeds
    private const float NEW_ENERGIES_SCALING_SPEED = 1.5f;

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

    public override void OnObjectDespawn() {
        // TODO Workaround for destructible objects list in OutScreenDestroyerController
        FixAddedToList();

        // Remove movement scripts
        RemoveMovementScripts();

        ObjectPool.SharedInstance.ReturnPooledObject(GetPoolType(), gameObject);
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
                    //GameObject positiveEnergy = ObjectPool.SharedInstance.SpawnPooledObject(positiveElementType);
                    GameObject positiveEnergy = SpawnPositiveEnergy();
                    positiveEnergy.transform.position = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * generatingPointPositive * transform.localScale.x + transform.position;
                    Vector3 distance = positiveEnergy.transform.position - transform.position;
                    // Change its random sizing script
                    RandomSize randomSizeScript = positiveEnergy.GetComponent<RandomSize>();
                    randomSizeScript.SetStartVarying(true);
                    randomSizeScript.SetScalingSpeed(NEW_ENERGIES_SCALING_SPEED);

                    // Get linear speed of asteroid's rotation
                    float linearSpeed = GetComponent<RotatingObject>().GetSpeed() / 360 * 2 * Mathf.PI * transform.localScale.x;

                    Vector3 perpendicularVector = new Vector3(-distance.y, distance.x, 0) * linearSpeed;
                    positiveEnergy.GetComponent<IMovingObject>().SetSpeed(distance.normalized * PlayerController.controller.GetSpeed() + perpendicularVector + GetComponent<IMovingObject>().GetSpeed());

                    //GameObject negativeEnergy = ObjectPool.SharedInstance.SpawnPooledObject(negativeElementType);
                    GameObject negativeEnergy = SpawnNegativeEnergy();
                    negativeEnergy.transform.position = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * generatingPointNegative * transform.localScale.x + transform.position;
                    distance = negativeEnergy.transform.position - transform.position;
                    // Change its random sizing script
                    randomSizeScript = negativeEnergy.GetComponent<RandomSize>();
                    randomSizeScript.SetStartVarying(true);
                    randomSizeScript.SetScalingSpeed(NEW_ENERGIES_SCALING_SPEED);

                    perpendicularVector = new Vector3(-distance.y, distance.x, 0) * linearSpeed;
                    negativeEnergy.GetComponent<IMovingObject>().SetSpeed(distance.normalized * PlayerController.controller.GetSpeed() + perpendicularVector + GetComponent<IMovingObject>().GetSpeed());

                    //Debug.Break();
                    spriteRenderer.material = idleMaterial;

                    SetState(IDLE);

                    // If genesis asteroid is non removable off screen, pass that attribute to the generated energies
                    if (!IsDestructibleNow()) {
                        DestructibleObject positiveDestructible = positiveEnergy.GetComponent<DestructibleObject>();
                        positiveDestructible.SetIsDestructibleNow(false);
                        DestructibleObject negativeDestructible = negativeEnergy.GetComponent<DestructibleObject>();
                        negativeDestructible.SetIsDestructibleNow(false);

                        StartCoroutine(AllowEnergiesRemovalOffscreen(new DestructibleObject[] { positiveDestructible, negativeDestructible }));
                    } 
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

    GameObject SpawnPositiveEnergy() {
        if (GetPoolType() == ElementsEnum.GENESIS_ASTEROID) {
            return ObjectPool.SharedInstance.SpawnPooledObject(ElementsEnum.POSITIVE_ENERGY);
        } else {
            return BackgroundStateController.controller.GetDistantForegroundGenerator()
                .GenerateSpecificDistantForegroundElement(ElementsEnum.DF_POSITIVE_ENERGY, Vector3.zero, BackgroundLayerEnum.SlowestDistantForegroundLayer);
        }
    }

    GameObject SpawnNegativeEnergy() {
        if (GetPoolType() == ElementsEnum.GENESIS_ASTEROID) {
            return ObjectPool.SharedInstance.SpawnPooledObject(ElementsEnum.NEGATIVE_ENERGY);
        }
        else {
            return BackgroundStateController.controller.GetDistantForegroundGenerator()
                .GenerateSpecificDistantForegroundElement(ElementsEnum.DF_NEGATIVE_ENERGY, Vector3.zero, BackgroundLayerEnum.SlowestDistantForegroundLayer);
        }
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

    IEnumerator AllowEnergiesRemovalOffscreen(DestructibleObject[] energies) {
        yield return new WaitForSeconds(5f);
        foreach (DestructibleObject energy in energies) {
            energy.SetIsDestructibleNow(true);
        }
    }


    // Remove energy movement scripts
    private void RemoveMovementScripts() {
        IMovingObject movingScript = GetComponent<IMovingObject>();
        movingScript.enabled = false;
    }

    void SetState(int state) {
        this.state = state;
        currentPointInCycle = 0;
    }
}
