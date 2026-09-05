using Godot;
using System;

public partial class UIResourceItem : HBoxContainer
{
    public int Id { get; set; }

    public TextureRect Icon { get; set; }

    public Label Value { get; set; }

    public override void _Ready()
    {
        Icon = GetNode<TextureRect>(nameof(Icon));
        Value = GetNode<Label>(nameof(Value));
    }
}
