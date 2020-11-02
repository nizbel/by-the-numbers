using UnityEngine;

public class RandomSize : MonoBehaviour
{
    private const float DEFAULT_MAX_SCALE = 1f;
    private const float DEFAULT_MIN_SCALE = 0.75f;

    [SerializeField]
    private float minScale = DEFAULT_MIN_SCALE;
    [SerializeField]
    private float maxScale = DEFAULT_MAX_SCALE;

    // Start is called before the first frame update
    void Awake()
    {
        float randomScale = Random.Range(minScale, maxScale);
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        Destroy(this);
    }
}
