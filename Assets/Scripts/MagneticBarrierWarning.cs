using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticBarrierWarning : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    TimedDurationObject durationScript;

    // Sprites and materials for warning
    [SerializeField]
    protected Sprite positiveSprite;
    [SerializeField]
    protected Material positiveMaterial;
    [SerializeField]
    protected Sprite negativeSprite;
    [SerializeField]
    protected Material negativeMaterial;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        durationScript = GetComponent<TimedDurationObject>();
    }

    public void SetIsPositiveWarning(bool isPositiveWarning) {

        if (isPositiveWarning) {
            spriteRenderer.sprite = positiveSprite;
            spriteRenderer.material = positiveMaterial;
        }
        else {
            spriteRenderer.sprite = negativeSprite;
            spriteRenderer.material = negativeMaterial;
        }
    }
}
