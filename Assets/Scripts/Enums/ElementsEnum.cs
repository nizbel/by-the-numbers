using UnityEngine;

[SerializeField]
public enum ElementsEnum
{
    // Foreground
    POSITIVE_ENERGY = 1,
    NEGATIVE_ENERGY,
    DEBRIS,
    ASTEROID,
    MAGNETIC_BARRIER,
    ENERGY_MINE,
    ENERGY_FUSE,
    STRAY_ENGINE,
    GENESIS_ASTEROID,

    // Background
    STAR = 21,
    GALAXY,
    BG_DEBRIS,
    DF_POSITIVE_ENERGY,
    DF_NEGATIVE_ENERGY,
    DF_DEBRIS,

    // UI
    ENERGY_INFLUENCE = 51
}