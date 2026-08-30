using Godot;
using System;

public partial class NavigationRegionController : NavigationRegion2D
{
    
    public static NavigationRegionController Instance { get; private set; }

    public override void _Ready()
    {
        if (!IsInstanceValid(Instance))
        {
            Instance = this;
        }
    }

    public bool IsPointInside(Vector2 point)
    {
        if (NavigationPolygon == null) return false;

        for (int i = 0; i < NavigationPolygon.GetPolygonCount(); i++)
        {
            var navPolygon = NavigationPolygon.GetPolygon(i);
            Vector2[] polygon = new Vector2[navPolygon.Length];

            for (int j = 0; j < navPolygon.Length; j++)
            {
                polygon[j] = NavigationPolygon.GetVertices()[navPolygon[j]];
            }

            if (Geometry2D.IsPointInPolygon(point, polygon))
            {
                return true;
            }
        }

        return false;
    }
}
