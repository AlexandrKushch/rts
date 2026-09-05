using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ResourceController : Node2D
{
    public Dictionary<int, int> CollectedResources { get; private set; }

    [Export] public ResourceType[] AvailableResources { get; private set; }

    public static ResourceController Instance { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        if (!IsInstanceValid(Instance))
        {
            Instance = this;
        }

        CollectedResources = AvailableResources.ToDictionary(x => x.Id, x => x.DefaultValue);
    }

    public void Collect(int resourceId, int value)
    {
        CheckResource(resourceId);

        CollectedResources[resourceId] += value;
    }

    public bool CheckSpent(int resourceId, int value)
    {
        CheckResource(resourceId);

        return CollectedResources[resourceId] - value > 0;
    }

    public void Spent(int resourceId, int value)
    {
        CheckResource(resourceId);
        CollectedResources[resourceId] -= value;
    }

    private void CheckResource(int resourceId)
    {
        if (!CollectedResources.ContainsKey(resourceId))
        {
            throw new NullReferenceException($"Resource with id {resourceId} nout found");
        }        
    }
}
