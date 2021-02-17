using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackElementEvent : ForegroundEvent
{
    ElementsEnum[] elementsAvailable;

    public BackElementGenerator.AmountEnum amount;

    [SerializeField]
    GameObject backElementGeneratorPrefab;

    public void SetElementsAvailable(ElementsEnum[] elementsAvailable) {
        this.elementsAvailable = elementsAvailable;
    }

    public void SetAmount(BackElementGenerator.AmountEnum amount) {
        this.amount = amount;
    }

    protected override void StartEvent() {
        // Spawn generator
        BackElementGenerator newGenerator = GameObject.Instantiate(backElementGeneratorPrefab).GetComponent<BackElementGenerator>();
        newGenerator.SetElementsAvailable(elementsAvailable);
        newGenerator.SetAmount(amount);

        // Disappear
        Destroy(gameObject);
    }
}
