using UnityEngine;

public class RandomSize : MonoBehaviour
{
    private const float DEFAULT_MAX_SCALE = 1f;
    private const float DEFAULT_MIN_SCALE = 0.75f;

    [SerializeField]
    private float minScale = DEFAULT_MIN_SCALE;
    [SerializeField]
    private float maxScale = DEFAULT_MAX_SCALE;

    bool startVarying = false;
    float randomScale;
    float startingScale = 0.1f;
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
            transform.localScale = new Vector3(startingScale, startingScale, 1);
        } else {
            Destroy(this);
        }
    }

    void FixedUpdate() {
        float currentScale = Mathf.Min(randomScale, Mathf.Lerp(transform.localScale.x, randomScale + 0.1f, Time.deltaTime * scalingSpeed)); 
        transform.localScale = new Vector3(currentScale, currentScale, 1);

        if (currentScale == randomScale) {
            Destroy(this);
        }
    }

    /*
     * Getters and Setters
     */
    public void SetStartVarying(bool startVarying) {
        this.startVarying = startVarying;
    }

    public void SetScalingSpeed(float scalingSpeed) {
        this.scalingSpeed = scalingSpeed;
    }
}
