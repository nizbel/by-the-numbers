using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingStar : MonoBehaviour
{
    float fadingFactor;

    SpriteRenderer spriteRenderer = null;

    float fadingStart = 0;

    private void Start() {
        fadingFactor = Random.Range(0.25f, 0.75f);

        spriteRenderer = GetComponent<SpriteRenderer>();

        fadingStart = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        Color color = spriteRenderer.color;
        color.a -= Time.deltaTime * fadingFactor;
        if (color.a <= 0) {
            Destroy(gameObject);
        }
        else {
            spriteRenderer.color = color;
        }
    }
}
