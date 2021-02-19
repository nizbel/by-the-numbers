using UnityEngine;

[SerializeField]
public enum MomentTypeEnum {
    // Unplayable
    Cutscene = 1,
    // Playable
    Gameplay,
    // Unplayable, used for observing constellations
    Constellation
}
