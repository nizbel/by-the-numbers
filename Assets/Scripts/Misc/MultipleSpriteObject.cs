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

    // Update is called once per frame
    void Update()
    {
        
    }

    public SpriteRenderer GetBiggestSpriteRenderer() {
        return biggestSpriteRenderer;
    }
}
