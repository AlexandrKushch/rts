using Godot;

public partial class PlayerBase : Node
{
    [Export] public TeamType Team { get; set; }

    private PlayerGlobalResources _globalResources;
    public PlayerGlobalResources GlobalResources
    {
        get
        {
            if (_globalResources == null)
            {
                _globalResources = GetNode<PlayerGlobalResources>(nameof(GlobalResources));
            }

            return _globalResources;
        }
    }
}
