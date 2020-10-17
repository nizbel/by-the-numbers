using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation : MonoBehaviour
{
    [SerializeField]
    protected float screenOffset = 0;

    // TODO manage charges somewhere else
    [SerializeField]
    protected int chargesAmount = 1;

    // Start is called before the first frame update
    void Start()
    {
        // TODO Get formation speed
        
        // TODO Add all direct children to destructible list settings own speed as theirs

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual float GetScreenOffset() {
        return screenOffset;
    }

    public int GetChargesAmount() {
        return chargesAmount;
    }

}
