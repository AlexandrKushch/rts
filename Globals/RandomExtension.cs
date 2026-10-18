using Godot;
using System;

public static class RandomExtension
{
    private static Random _random = new Random();

    public static long RandomLong(int maxValue)
    {
        return _random.NextInt64(maxValue);
    }

    public static double RandomDouble()
    {
        return _random.NextDouble();
    }

    public static double RandomDouble(double maxValue)
    {
        return _random.NextDouble() * maxValue;
    }

    public static Vector2 GetRandomPointInCircle(float radius)
    {
        float angle = (float)_random.NextDouble() * Mathf.Pi * 2;
        float r = radius * (float)Mathf.Sqrt(_random.NextDouble());
        float x = r * Mathf.Cos(angle);
        float y = r * Mathf.Sin(angle);

        return new Vector2(x, y);
    }

    public static Vector2 GetRandomPointInRect(float w, float h)
    {
        return new Vector2((float)RandomDouble(w), (float)RandomDouble(h));
    }

    public static Vector2 GetRandomPointInRect(RectangleShape2D shape)
    {
        return GetRandomPointInRect(shape.Size.X, shape.Size.Y) - (new Vector2(shape.Size.X, shape.Size.Y) / 2);
    }

    public static Vector2 GetRandomPointOnCircle(float radius)
    {
        float angle = (float)_random.NextDouble() * Mathf.Pi * 2;
        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);

        return new Vector2(x, y);        
    }
}
