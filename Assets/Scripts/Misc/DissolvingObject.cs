using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DissolvingObject : MonoBehaviour {

    private const float DEFAULT_DISSOLVE_SPEED = 2f;

    float dissolveSpeed = DEFAULT_DISSOLVE_SPEED;

    SpriteRenderer spriteRenderer;

    // Keeps the initial value for dissolve amount
    float initialDissolveAmount;

    // Workaround to destroy material instantiated
    Material dissolvingParticlesMaterial = null;

    void Awake() {
        // TODO Make this work for multiple sprite object (if needed)
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start() {
        // If dissolution by damage (dissolveSpeed == 0), object can still collider around
        if (dissolveSpeed > 0) {
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders) { 
                collider.enabled = false;
            }
        }

        initialDissolveAmount = spriteRenderer.material.GetFloat("_DissolveAmount");

        // TODO Change place of seeding
        // Set random offset for dissolve texture start
        if (initialDissolveAmount == 0) {
            spriteRenderer.material.SetVector("_Seed", new Vector4(Random.Range(0, 1f), Random.Range(0, 1f), 0, 0));
        }
    }

    // Update is called once per frame
    void Update() {
        if (dissolveSpeed > 0) {
            // Animate dissolution
            float dissolveAmount = Mathf.Lerp(spriteRenderer.material.GetFloat("_DissolveAmount"), 1, dissolveSpeed * Time.deltaTime);
            spriteRenderer.material.SetFloat("_DissolveAmount", dissolveAmount);
        }
    }

    public void SetDissolutionByEnergy(int value) {
        // Add dissolving particles
        ParticleSystem dissolvingParticles = GameObject.Instantiate(StageController.controller.dissolvingParticlesPrefab, transform).GetComponent<ParticleSystem>();

        ParticleSystemRenderer dissolvingParticlesRenderer = dissolvingParticles.GetComponent<ParticleSystemRenderer>();
        ParticleSystem.TextureSheetAnimationModule dissolvingParticlesSheet = dissolvingParticles.textureSheetAnimation;
        dissolvingParticlesSheet.AddSprite(spriteRenderer.sprite);

        if (value < 0) {
            // Sprite
            spriteRenderer.material.SetColor("_DissolveColorOuter", new Color(8.5f, 0, 0, 1));
            spriteRenderer.material.SetColor("_DissolveColorMiddle", new Color(6.5f, 3f, 0, 1));
            spriteRenderer.material.SetColor("_DissolveColorInner", new Color(4.5f, 6f, 0, 1));

            // Dissolving particles
            dissolvingParticlesRenderer.material.SetColor("_DissolveColorOuter", new Color(8.5f, 0, 0, 1));
            dissolvingParticlesRenderer.material.SetColor("_DissolveColorMiddle", new Color(6.5f, 3f, 0, 1));
            dissolvingParticlesRenderer.material.SetColor("_DissolveColorInner", new Color(4.5f, 6f, 0, 1));
        } else {
            // Sprite
            spriteRenderer.material.SetColor("_DissolveColorOuter", new Color(0, 4f, 8.5f, 1));
            spriteRenderer.material.SetColor("_DissolveColorMiddle", new Color(0, 3f, 6.5f, 1));
            spriteRenderer.material.SetColor("_DissolveColorInner", new Color(0, 2f, 4.5f, 1));

            // Dissolving particles
            dissolvingParticlesRenderer.material.SetColor("_DissolveColorOuter", new Color(0, 4f, 8.5f, 1));
            dissolvingParticlesRenderer.material.SetColor("_DissolveColorMiddle", new Color(0, 3f, 6.5f, 1));
            dissolvingParticlesRenderer.material.SetColor("_DissolveColorInner", new Color(0, 2f, 4.5f, 1));
        }

        // If objects is debris, stop fragments
        Debris debrisScript = GetComponent<Debris>();
        if (debrisScript != null) {
            debrisScript.DisappearFragments();
        }

        // Remove shadow caster if exists
        ShadowCaster2D shadowCaster = GetComponent<ShadowCaster2D>();
        if (shadowCaster != null) {
            shadowCaster.enabled = false;
        }

        dissolvingParticles.Play();
    }

    public void SetDissolutionByDamage() {
        // Change dissolution amount as to show damage in object
        dissolveSpeed = 0;

        spriteRenderer.material.SetFloat("_DissolveAmount", Random.Range(0.5f, 0.6f));

        // Add dissolving particles
        ParticleSystem dissolvingParticles = GameObject.Instantiate(StageController.controller.dissolvingParticlesPrefab, transform).GetComponent<ParticleSystem>();
        ParticleSystem.TextureSheetAnimationModule dissolvingParticlesSheet = dissolvingParticles.textureSheetAnimation;
        dissolvingParticlesSheet.AddSprite(spriteRenderer.sprite);
        dissolvingParticles.Play();
    }

    private void OnDisable() {
        // Return dissolve amount to initial value
        spriteRenderer.material.SetFloat("_DissolveAmount", initialDissolveAmount);
        // Set color as transparent
        spriteRenderer.material.SetColor("_DissolveColorOuter", new Color(1, 1, 1, 0));
        spriteRenderer.material.SetColor("_DissolveColorMiddle", new Color(1, 1, 1, 0));
        spriteRenderer.material.SetColor("_DissolveColorInner", new Color(1, 1, 1, 0));
    }
}
