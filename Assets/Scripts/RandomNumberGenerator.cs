using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomNumberGenerator
{
    public static int GenerateWeightedRandomNumber(List<int> numbers, List<float> weights)
    {
        if (numbers.Count != weights.Count)
        {
            throw new System.ArgumentException("RNG - Numbers and weights must have the same length");
        }

        // Calculate the total weight
        float totalWeight = 0f;
        foreach (float weight in weights)
        {
            totalWeight += weight;
        }

        // Generate a random value between 0 and totalWeight
        float randomValue = Random.Range(0, totalWeight);

        // Find the corresponding number based on the random value
        float cumulativeWeight = 0f;
        for (int i = 0; i < weights.Count; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue < cumulativeWeight)
            {
                return numbers[i];
            }
        }

        return numbers[numbers.Count - 1]; // Fallback in case of rounding errors
    }
}

