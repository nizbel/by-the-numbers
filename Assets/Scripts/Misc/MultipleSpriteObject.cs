using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleSpriteObject : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer biggestSpriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public SpriteRenderer GetBiggestSpriteRenderer() {
        return biggestSpriteRenderer;
    }
}
