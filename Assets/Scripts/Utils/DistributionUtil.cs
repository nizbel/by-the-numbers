using UnityEngine;

public static class DistributionUtil
{
    public static float GetNormalDistribution(float minValue, float maxValue) {
        float averageValue = (minValue + maxValue) / 2;
        return Random.Range(0, averageValue - minValue) + Random.Range(0, averageValue - minValue) + minValue;
    }
}
