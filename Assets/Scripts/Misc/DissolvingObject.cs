using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolvingObject : MonoBehaviour {

    private const float DEFAULT_DISSOLVE_SPEED = 2f;

    float dissolveSpeed = DEFAULT_DISSOLVE_SPEED;

    SpriteRenderer spriteRenderer;

    void Awake() {
        // TODO Make this work for multiple sprite object (if needed)
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start() {
        GetComponent<Collider2D>().enabled = false;

        // TODO Set random offset for dissolve texture start

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
        if (value < 0) {
            spriteRenderer.material.SetColor("_DissolveColorOuter", new Color(8.5f, 0, 0, 1));
            spriteRenderer.material.SetColor("_DissolveColorMiddle", new Color(6.5f, 3f, 0, 1));
            spriteRenderer.material.SetColor("_DissolveColorInner", new Color(4.5f, 6f, 0, 1));
        } else {
            spriteRenderer.material.SetColor("_DissolveColorOuter", new Color(0, 4f, 8.5f, 1));
            spriteRenderer.material.SetColor("_DissolveColorMiddle", new Color(0, 3f, 6.5f, 1));
            spriteRenderer.material.SetColor("_DissolveColorInner", new Color(0, 2f, 4.5f, 1));
        }
    }

    public void SetDissolutionByDamage() {
        // Change dissolution amount as to show damage in object
        dissolveSpeed = 0;

        spriteRenderer.material.SetFloat("_DissolveAmount", Random.Range(0.35f, 0.45f));
    }

}
