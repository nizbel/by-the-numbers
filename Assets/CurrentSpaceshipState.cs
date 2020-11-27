using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentSpaceshipState : MonoBehaviour
{
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

    // Update is called once per frame
    void Update()
    {
        
    }


    void SetCurrentShipSpriteByDay(int currentDay) {
        // TODO Alter these values
        if (currentDay > 80) {
            spriteRenderer.sprite = sprites[3];
        }
        else if (currentDay > 20) {
            spriteRenderer.sprite = sprites[2];
        }
        else if (currentDay > 7) {
            spriteRenderer.sprite = sprites[1];
        }
        // If none of the above, keep the default sprite in the sheet
    }
}
