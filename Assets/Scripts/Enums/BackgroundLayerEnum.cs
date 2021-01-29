using UnityEngine;

[SerializeField]
public enum BackgroundLayerEnum {
    // Background
    RandomBackgroundLayer,
    RandomMovingBackgroundLayer,
    SlowestBackgroundLayer,
    FastestBackgroundLayer,

    // Distant foreground
    RandomDistantForegroundLayer,
    SlowestDistantForegroundLayer,
    FastestDistantForegroundLayer
}
