using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation : MonoBehaviour
{
    [SerializeField]
    protected float screenOffset = 0;

    void Awake() {
        // Update ForegroundEvent script
        GetComponent<ForegroundEvent>().SetCooldown(0.1f * transform.childCount);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual float GetScreenOffset() {
        return screenOffset;
    }
}
