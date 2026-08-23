using Godot;
using Globals;

public partial class CameraController : Node2D
{
	private const float Speed = 1000;

	private bool _wasdLocked = false;
	private float _zoom = 0.5f;

	private Vector2 _dragStart;
	private Vector2 _dragEnd;

	[Export]
	private float FollowSpeed = 10;
	
	[Export]
	private float ZoomSpeed = 7.5f;

	[Export]
	private Camera2D Camera;

	public override void _PhysicsProcess(double delta)
	{
		ProcessDrag((float)delta);
		ProcessWasd((float)delta);
		ProcessZoom((float)delta);
	}

	private void ProcessDrag(float delta)
	{
		if (Input.IsActionJustPressed(InputMapGlobal.Mmb))
		{
			_wasdLocked = true;
			_dragStart = GetGlobalMousePosition();
			_dragEnd = GetGlobalMousePosition();
		}
		else if (Input.IsActionPressed(InputMapGlobal.Mmb))
		{
			_dragEnd = GetGlobalMousePosition();
			Camera.Position = Camera.Position.Lerp(Camera.Position + (_dragStart - _dragEnd), delta * FollowSpeed);
		}
		else if (Input.IsActionJustReleased(InputMapGlobal.Mmb))
		{
			_wasdLocked = false;
			Position = Camera.Position;
		}
	}

	private void ProcessWasd(float delta)
	{
		if (_wasdLocked) return;

		var direction = Input.GetVector(
				InputMapGlobal.Left,
				InputMapGlobal.Right,
				InputMapGlobal.Up,
				InputMapGlobal.Down)
			.Normalized();

		direction *= 10 + Mathf.Remap(_zoom, 0, 1, +10, -5);

		Position = Position.MoveToward(Position + direction, Speed * delta);

		Camera.Position = Camera.Position.Lerp(Position, delta * FollowSpeed);        
	}

	private void ProcessZoom(float delta)
	{
		if (Input.IsActionJustReleased(InputMapGlobal.ZoomIn))
		{
			_zoom += 0.1f;
		}
		else if (Input.IsActionJustReleased(InputMapGlobal.ZoomOut))
		{
			_zoom -= 0.1f;
		}
		
		_zoom = Mathf.Clamp(_zoom, 0, 1);

		var newZoom = (float)Mathf.Remap(_zoom, 0, 1, 0.25, 2);

		Camera.Zoom = Camera.Zoom.Lerp(Vector2.One * newZoom, delta * ZoomSpeed);
	}
}
