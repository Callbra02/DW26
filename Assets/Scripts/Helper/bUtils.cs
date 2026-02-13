using System.Collections.Generic;
using UnityEngine;

// Helper class for various scripts
public static class bUtils
{
    // Helper function to normalize slider values between 0 and 1
    // Min and max variables are relative to the value that is being normalized
    // e.g. throw is a value between 0 and a given max || NSV(throwval, 0, throwvalmax)
    public static float NormalizeSliderValue(float value, float min, float max)
    {
        // Return 0 if applicable
        if (max == min)
            return 0.0f;

        // Else return the normalized value between 0 and 1
        return (value - min) / (max - min);
    }

    public static void RotateToDirection(Vector2 dir, Transform transform)
    {
        if (dir != Vector2.zero)
        {
            float a = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            float rot = a;

            transform.eulerAngles = new Vector3(0, 0, a);
        }
    }
}
