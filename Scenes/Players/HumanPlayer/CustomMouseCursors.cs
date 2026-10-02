using Godot;
using System.Collections.Generic;

public partial class CustomMouseCursors : Node
{
    public const int Default = 1;
    public const int Pointer = 2;
    public const int Error = 3;

    private Dictionary<int, CompressedTexture2D> _cursors;

    public int PointerCount { get; private set; } = 0;

    public static CustomMouseCursors Instance { get; private set; }

    public override void _Ready()
    {
        if (!IsInstanceValid(Instance))
        {
            Instance = this;
        }

        _cursors = new Dictionary<int, CompressedTexture2D>
        {
            [Default] = ResourceLoader.Load<CompressedTexture2D>("uid://blgc04gqaxp3s"),
            [Pointer] = ResourceLoader.Load<CompressedTexture2D>("uid://mgnktp5gehqs"),
            [Error] = ResourceLoader.Load<CompressedTexture2D>("uid://bcq0wm8s5m5up")
        };

        UpdateCursor(Default);
    }

    public void UpdateCursorPointerCount(int value)
    {
        PointerCount += value;

        if (PointerCount > 0)
        {
            UpdateCursor(Pointer);
        }
        else
        {
            UpdateCursor(Default);            
        }
    }

    public void UpdateCursor(int type)
    {
        if (_cursors.TryGetValue(type, out var texture))
        {
            Input.SetCustomMouseCursor(texture, hotspot: new Vector2(22, 17));
        }
    }
}
