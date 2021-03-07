using UnityEngine;

public class RandomSize : MonoBehaviour
{
    private const float DEFAULT_MAX_SCALE = 1f;
    private const float DEFAULT_MIN_SCALE = 0.75f;
    private const float DEFAULT_STARTING_SCALE = 0.01f;

    [SerializeField]
    private float minScale = DEFAULT_MIN_SCALE;
    [SerializeField]
    private float maxScale = DEFAULT_MAX_SCALE;

    bool startVarying = false;
    float randomScale;
    float startingScale = DEFAULT_STARTING_SCALE;
    float scalingSpeed = 1f;

    // Start is called before the first frame update
    void Awake()
    {
        randomScale = Random.Range(minScale, maxScale);
        transform.localScale = new Vector3(randomScale, randomScale, 1);

        // TODO Add mass variation through scale
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        if (rigidbody != null) {
            rigidbody.mass *= transform.localScale.x;
        }
    }

    void Start() {
        if (startVarying) {
            transform.localScale = Vector3.one * startingScale;
        } else {
            enabled = false;
        }
    }

    void Update() {
        //float currentScale = Mathf.Min(randomScale, Mathf.Lerp(transform.localScale.x, randomScale + 0.1f, Time.deltaTime * scalingSpeed)); 
        float currentScale = Mathf.MoveTowards(transform.localScale.x, randomScale, Time.deltaTime * scalingSpeed);
        transform.localScale = new Vector3(currentScale, currentScale, 1);

        if (currentScale == randomScale) {
            enabled = false;
        }
    }

    // Changes target and starting scale, best used before right at start
    public void MultiplyScales(float multiplier) {
        randomScale *= multiplier;
        startingScale *= multiplier;
        transform.localScale = startingScale * Vector3.one;

        // Enable to start scaling
        enabled = true;
    }

    /*
     * Getters and Setters
     */
    public bool GetStartVarying() {
        return startVarying;
    }

    public void SetStartVarying(bool startVarying) {
        this.startVarying = startVarying;
    }

    public void SetScalingSpeed(float scalingSpeed) {
        this.scalingSpeed = scalingSpeed;
    }

}
