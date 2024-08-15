using System;
using Unity.VisualScripting;
using UnityEngine;

public class RankCalculator : MonoBehaviour
{
    // Variables to set from the gameplay
    public static float leastFlipsRequired;
    public static float actualFlips; // Make it static
    private float optimalTime;  // Set this based on your game's design
    private float actualTime;

    // Variables to store the calculated values
    private float efficiencyRatio;
    private float timeFactor;
    private float compositeScore;

    public String CalculateRank(LevelData level, float actualTime)
    {
        optimalTime = level.optimalTimeS;
        Debug.Log("Optimal Time:" + optimalTime);

        Debug.Log("Min Flips:" + leastFlipsRequired);
        Debug.Log("Actual Flips:" + actualFlips);

        // Calculate Efficiency Ratio
        efficiencyRatio = (leastFlipsRequired / actualFlips) * 100;
        Debug.Log("Efficiency Ratio:" + efficiencyRatio);

        // Calculate Time Factor
        timeFactor = (optimalTime / actualTime) * 100;
        Debug.Log("Time Factor:" + timeFactor);

        // Calculate Composite Score
        compositeScore = (efficiencyRatio * 0.6f) + (timeFactor * 0.4f);
        Debug.Log("Composite Score:" + compositeScore);

        // Determine Rank
        if (compositeScore >= 90)
        {
            Debug.Log("Rank: S");
            return "S";
        }
        else if (compositeScore >= 80)
        {
            Debug.Log("Rank: A");
            return "A";
        }
        else if (compositeScore >= 70)
        {
            Debug.Log("Rank: B");
            return "B";
        }
        else if (compositeScore >= 60)
        {
            Debug.Log("Rank: C");
            return "C";
        }
        else
        {
            Debug.Log("Rank: D");
            return "D";
        }
    }

    public static void IncrementMinFlips() {
        leastFlipsRequired += 1;
    }

    public static void ResetMinFlips() {
        leastFlipsRequired = 0;
    }

    public static void IncrementFlips()
    {
        actualFlips += 1;
    }

    public static void ResetFlips()
    {
        actualFlips = 0;
    }

}