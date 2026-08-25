using Godot;
using System;

public static class RandomExtension
{
    private static Random _random = new Random();

    public static Vector2 GetRandomPointInCircle(float radius)
    {
        float angle = (float)_random.NextDouble() * Mathf.Pi * 2;
        float r = radius * (float)Mathf.Sqrt(_random.NextDouble());
        float x = r * Mathf.Cos(angle);
        float y = r * Mathf.Sin(angle);

        return new Vector2(x, y);
    }
}
