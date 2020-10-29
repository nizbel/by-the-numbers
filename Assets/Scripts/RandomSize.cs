using UnityEngine;

public class RandomSize : MonoBehaviour
{
    private const float MAX_SCALE = 1f;
    private const float MIN_SCALE = 0.75f;

    // Start is called before the first frame update
    void Awake()
    {
        float randomScale = Random.Range(MIN_SCALE, MAX_SCALE);
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        Destroy(this);
    }
}
