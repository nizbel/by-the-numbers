using System;
using UnityEngine;

[Serializable]
public class ElementSpawnChance {
    public ElementsEnum element;

    [Tooltip("Chance of spawning element, for magnetic barriers it just means it can spawn")]
    public float chance = -1;

    public ElementSpawnChance(ElementsEnum element, float chance) {
        this.element = element;
        this.chance = chance;
    }
}
