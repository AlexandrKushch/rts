using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ResourceController : Node2D
{
    public Dictionary<ResourceType, int> CollectedResources { get; private set; }

    [Export] public ResourceInfo[] AvailableResources { get; private set; }

    public static ResourceController Instance { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        if (!IsInstanceValid(Instance))
        {
            Instance = this;
        }

        CollectedResources = AvailableResources.ToDictionary(x => x.Type, x => x.DefaultValue);
    }

    public void Collect(ResourceType resourceType, int value)
    {
        CheckResource(resourceType);

        CollectedResources[resourceType] += value;
    }

    public bool CheckSpent(ResourceType resourceType, int value)
    {
        CheckResource(resourceType);
        return CollectedResources[resourceType] - value >= 0;
    }

    public void Spent(ResourceType resourceType, int value)
    {
        CheckResource(resourceType);
        CollectedResources[resourceType] -= value;
    }

    private void CheckResource(ResourceType resourceType)
    {
        if (!CollectedResources.ContainsKey(resourceType))
        {
            throw new NullReferenceException($"Resource with id {resourceType} not found");
        }        
    }
}
