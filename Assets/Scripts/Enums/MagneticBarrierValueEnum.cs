using UnityEngine;

[SerializeField]
public enum MagneticBarrierValueEnum {
    // Whether magnetic barriers should be positive or negative throughout the stage moment
    Random, // 50/50 between positive and negative during the moment
    PositiveOrNegative, // Choose one value and stick to it
    KeepValue, // Keep value that is already defined
    Positive,
    Negative
}
