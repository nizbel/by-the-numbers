using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolvingObject : MonoBehaviour {

    private const float DEFAULT_DISSOLVE_SPEED = 2f;

    float dissolveSpeed = DEFAULT_DISSOLVE_SPEED;

    SpriteRenderer spriteRenderer;

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
            GetComponent<Collider2D>().enabled = false;
        }

        // Set random offset for dissolve texture start
        spriteRenderer.material.SetVector("_Seed", new Vector4(Random.Range(0, 1f), Random.Range(0, 1f), 0, 0));

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
        // TODO Find a way to destroy these materials
        dissolvingParticlesMaterial = Instantiate(dissolvingParticles.GetComponent<ParticleSystemRenderer>().material);
        dissolvingParticles.GetComponent<ParticleSystemRenderer>().material = dissolvingParticlesMaterial;
        ParticleSystem.TextureSheetAnimationModule dissolvingParticlesSheet = dissolvingParticles.textureSheetAnimation;
        dissolvingParticlesSheet.AddSprite(spriteRenderer.sprite);

        if (value < 0) {
            // Sprite
            spriteRenderer.material.SetColor("_DissolveColorOuter", new Color(8.5f, 0, 0, 1));
            spriteRenderer.material.SetColor("_DissolveColorMiddle", new Color(6.5f, 3f, 0, 1));
            spriteRenderer.material.SetColor("_DissolveColorInner", new Color(4.5f, 6f, 0, 1));

            // Dissolving particles
            dissolvingParticlesMaterial.SetColor("_DissolveColorOuter", new Color(8.5f, 0, 0, 1));
            dissolvingParticlesMaterial.SetColor("_DissolveColorMiddle", new Color(6.5f, 3f, 0, 1));
            dissolvingParticlesMaterial.SetColor("_DissolveColorInner", new Color(4.5f, 6f, 0, 1));
        } else {
            // Sprite
            spriteRenderer.material.SetColor("_DissolveColorOuter", new Color(0, 4f, 8.5f, 1));
            spriteRenderer.material.SetColor("_DissolveColorMiddle", new Color(0, 3f, 6.5f, 1));
            spriteRenderer.material.SetColor("_DissolveColorInner", new Color(0, 2f, 4.5f, 1));

            // Dissolving particles
            dissolvingParticlesMaterial.SetColor("_DissolveColorOuter", new Color(0, 4f, 8.5f, 1));
            dissolvingParticlesMaterial.SetColor("_DissolveColorMiddle", new Color(0, 3f, 6.5f, 1));
            dissolvingParticlesMaterial.SetColor("_DissolveColorInner", new Color(0, 2f, 4.5f, 1));
        }

        dissolvingParticles.Play();
    }

    public void SetDissolutionByDamage() {
        // Change dissolution amount as to show damage in object
        dissolveSpeed = 0;

        spriteRenderer.material.SetFloat("_DissolveAmount", Random.Range(0.5f, 0.6f));

        // Add dissolving particles
        ParticleSystem dissolvingParticles = GameObject.Instantiate(StageController.controller.dissolvingParticlesPrefab, transform).GetComponent<ParticleSystem>();
        dissolvingParticlesMaterial = Instantiate(dissolvingParticles.GetComponent<ParticleSystemRenderer>().material);
        dissolvingParticles.GetComponent<ParticleSystemRenderer>().material = dissolvingParticlesMaterial;
        ParticleSystem.TextureSheetAnimationModule dissolvingParticlesSheet = dissolvingParticles.textureSheetAnimation;
        dissolvingParticlesSheet.AddSprite(spriteRenderer.sprite);
        dissolvingParticles.Play();
    }

    private void OnDisable() {
        Destroy(dissolvingParticlesMaterial);
    }
}
