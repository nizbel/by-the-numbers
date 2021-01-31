using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentSpaceshipState : MonoBehaviour
{
    public const int DAY_FOR_SECOND_SPRITE = 16;
    public const int DAY_FOR_THIRD_SPRITE = 46;
    public const int DAY_FOR_FOURTH_SPRITE = 81;

    [SerializeField]
    Sprite[] sprites;

    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Define current sprite depending on day
        SetCurrentShipSpriteByDay(GameController.controller.GetCurrentDay());

        Destroy(this);
    }

    void SetCurrentShipSpriteByDay(int currentDay) {
        // TODO Find a way to get current spaceship damage dynamically

        // TODO Alter these values
        if (currentDay >= DAY_FOR_FOURTH_SPRITE) {
            spriteRenderer.sprite = sprites[3];
            spriteRenderer.material.SetFloat("_CurrentDamageSprite", 0);
            PlayerController.controller.GetBurningAnimationSpriteRenderer().sprite = sprites[3];
        }
        else if (currentDay >= DAY_FOR_THIRD_SPRITE) {
            // Reset damage counter
            if (currentDay == DAY_FOR_THIRD_SPRITE) {
                GameController.SetShipDamage(0);
            }
            spriteRenderer.sprite = sprites[2];
            spriteRenderer.material.SetFloat("_CurrentDamage", Mathf.Max((float)(currentDay - DAY_FOR_THIRD_SPRITE) / (DAY_FOR_FOURTH_SPRITE - DAY_FOR_THIRD_SPRITE), GameController.GetShipDamage()));
            PlayerController.controller.GetBurningAnimationSpriteRenderer().sprite = sprites[2];
        }
        else if (currentDay >= DAY_FOR_SECOND_SPRITE) {
            // Reset damage counter
            if (currentDay == DAY_FOR_SECOND_SPRITE) {
                GameController.SetShipDamage(0);
            }
            spriteRenderer.sprite = sprites[1];
            spriteRenderer.material.SetFloat("_CurrentDamage", Mathf.Max((float)(currentDay - DAY_FOR_SECOND_SPRITE) / (DAY_FOR_THIRD_SPRITE - DAY_FOR_SECOND_SPRITE), GameController.GetShipDamage()));
            PlayerController.controller.GetBurningAnimationSpriteRenderer().sprite = sprites[1];
        }
        else {
            // If none of the above, keep the default sprite in the sheet
            spriteRenderer.material.SetFloat("_CurrentDamage", Mathf.Max((float)(currentDay - 1) / (DAY_FOR_SECOND_SPRITE-1), GameController.GetShipDamage()));
        }

        // Set seed for damage material
        Vector2 seed = GameController.GetShipDamageSeed();
        spriteRenderer.material.SetVector("_DamageSeed", new Vector4(seed.x, seed.y, 0, 0));
    }
}
