using UnityEngine;

public class BlinkingSprite : MonoBehaviour
{
    private const float DEFAULT_FREQUENCY = 0.5f;

    private float createdAt;

    [SerializeField]
    private float blinkFrequency = DEFAULT_FREQUENCY;

    private float lastBlink;

    [SerializeField]
    private bool changeSprites = false;

    [SerializeField]
    private Sprite[] sprites = null;
    private int currentSprite = 0;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        createdAt = Time.time;
        lastBlink = createdAt;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastBlink > blinkFrequency) {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            if (changeSprites && spriteRenderer.enabled) {
                currentSprite = (currentSprite+1) % sprites.Length;
                spriteRenderer.sprite = sprites[currentSprite];
            }
            lastBlink = Time.time;
        }
    }
}
