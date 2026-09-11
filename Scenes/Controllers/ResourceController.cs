using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ResourceController : Node2D
{
    public Dictionary<ResourceTypeIds, int> CollectedResources { get; private set; }

    public static ResourceController Instance { get; private set; }

    [Signal]
    public delegate void ChangedEventHandler();

    public override void _Ready()
    {
        base._Ready();

        if (!IsInstanceValid(Instance))
        {
            Instance = this;
        }

        CollectedResources = GlobalResources.Instance.GatheringResources.ToDictionary(x => x.Key, x => x.Value.DefaultValue);
    }

    public void Collect(ResourceTypeIds resourceType, int value)
    {
        CheckResource(resourceType);
        CollectedResources[resourceType] += value;
        EmitSignal(SignalName.Changed);
    }

    public bool CheckSpent(ResourceTypeIds resourceType, int value)
    {
        CheckResource(resourceType);
        return CollectedResources[resourceType] - value >= 0;
    }

    public void Spent(ResourceTypeIds resourceType, int value)
    {
        CheckResource(resourceType);
        CollectedResources[resourceType] -= value;
        EmitSignal(SignalName.Changed);
    }

    private void CheckResource(ResourceTypeIds resourceType)
    {
        if (!CollectedResources.ContainsKey(resourceType))
        {
            throw new NullReferenceException($"Resource with id {resourceType} not found");
        }        
    }
}
