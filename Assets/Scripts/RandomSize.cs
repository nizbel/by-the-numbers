using UnityEngine;

public class RandomSize : MonoBehaviour
{
    private const float DEFAULT_MAX_SCALE = 1f;
    private const float DEFAULT_MIN_SCALE = 0.75f;
    private const float DEFAULT_STARTING_SCALE = 0.25f;
    private const float DEFAULT_SCALING_SPEED = 1f;

    [SerializeField]
    private float minScale = DEFAULT_MIN_SCALE;
    [SerializeField]
    private float maxScale = DEFAULT_MAX_SCALE;

    bool startVarying = false;
    float randomScale;
    float startingScale = DEFAULT_STARTING_SCALE;
    float scalingSpeed = DEFAULT_SCALING_SPEED;

    // Start is called before the first frame update
    void Awake() {
        randomScale = Random.Range(minScale, maxScale);
        transform.localScale = randomScale * Vector3.one;

        // TODO Add mass variation through scale
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        if (rigidbody != null) {
            rigidbody.mass *= transform.localScale.x;
        }
    }

    private void OnEnable() {
        // Return object to its randomly chosen scale
        if (startVarying) {
            transform.localScale = startingScale * Vector3.one;
        }
        else {
            transform.localScale = randomScale * Vector3.one;
            enabled = false;
        }
    }

    private void OnDisable() {
        // Deactivate start varying 
        startVarying = false;
    }

    void Update() {
        float currentScale = Mathf.MoveTowards(transform.localScale.x, randomScale, Time.deltaTime * scalingSpeed);
        transform.localScale = new Vector3(currentScale, currentScale, currentScale);

        if (currentScale == randomScale) {
            enabled = false;
        }
    }

    // Changes target and starting scale, best used before right at start
    public void MultiplyScales(float multiplier) {
        randomScale *= multiplier;
        startingScale *= multiplier;

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
        // If element's size is supposed to vary, re-enable it
        if (startVarying) {
            enabled = true;
            transform.localScale *= startingScale;
        }
    }

    public void SetScalingSpeed(float scalingSpeed) {
        this.scalingSpeed = scalingSpeed;
    }

    // TODO Remove
    //void Log(string method) {
    //    Debug.Log(method + ": " + gameObject.GetInstanceID() + "... Scale: " + transform.localScale + "... Random scale: " + randomScale);
    //} 
}
