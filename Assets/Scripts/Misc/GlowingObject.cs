using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class GlowingObject : MonoBehaviour
{
    [SerializeField]
    Light2D glowingLight = null;

    float defaultGlowRadius;

    [SerializeField]
    float glowSpeed = 2f;

    [SerializeField]
    float maxMultiplier = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        defaultGlowRadius = glowingLight.pointLightOuterRadius;
    }

    // Update is called once per frame
    void Update() {
        if (glowingLight.pointLightOuterRadius <= defaultGlowRadius) {
            glowSpeed = Mathf.Abs(glowSpeed);
        }
        else if (glowingLight.pointLightOuterRadius > maxMultiplier * defaultGlowRadius) {
            glowSpeed = -1 * Mathf.Abs(glowSpeed);
        }
        glowingLight.pointLightOuterRadius = Mathf.Lerp(glowingLight.pointLightOuterRadius, glowingLight.pointLightOuterRadius + glowSpeed, Time.deltaTime);
    }
}
