using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistributionUtil : MonoBehaviour
{
    public static float GetNormalDistribution(float minValue, float maxValue) {
        float averageValue = (minValue + maxValue) / 2;
        return Random.Range(0, averageValue - minValue) + Random.Range(0, averageValue - minValue) + minValue;
    }
}
