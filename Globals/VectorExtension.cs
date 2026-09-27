using Godot;

public static class VectorExtension
{
    public static bool IsEqualApprox(this Vector2 a, Vector2 b, float tolerance = 0.1f)
    {
        if (Mathf.IsEqualApprox(a.X, b.X, tolerance))
        {
            return Mathf.IsEqualApprox(a.Y, b.Y, tolerance);
        }

        return false;
    }
}
