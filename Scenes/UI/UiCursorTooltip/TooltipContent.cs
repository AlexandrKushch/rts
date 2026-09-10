using Godot.Collections;

public class TooltipContent
{
    public string Title { get; set; }

    public Dictionary<ResourceTypeIds, int> ResourcesCost { get; set; }

    public string Description { get; set; }
}