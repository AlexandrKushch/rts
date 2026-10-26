using Godot;
using Globals;

public partial class CameraController : Node2D
{
	private const float Speed = 1000;

	private bool _wasdLocked = false;
	private float _zoom = 0.5f;
	private float _zoomRatio = 0.5f;

	private Vector2 _dragStart;
	private Vector2 _dragEnd;

	[Export]
	private float FollowSpeed = 20;
	
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

	public override void _UnhandledInput(InputEvent input)
	{
		if (input is InputEventMouseButton inputButton)
		{
			if (inputButton.ButtonIndex == MouseButton.WheelUp)
			{
				_zoom += 0.1f * _zoomRatio;
			}
			else if (inputButton.ButtonIndex == MouseButton.WheelDown)
			{
				_zoom -= 0.1f * _zoomRatio;
			}
		}
	}

    public void AlignWith(Vector2 position)
    {
		// GlobalPosition = position + Vector2.Up * 200;
		var tween = CreateTween();
		tween.TweenProperty(this, "position", position + Vector2.Up * 200, 0.2f);
    }

	// public void Shake()
	// {
	// 	var tween = CreateTween().SetParallel();

	// 	var tweenZoom = CreateTween().SetTrans(Tween.TransitionType.Bounce);
	// 	var originalZoom = Camera.Zoom;
	// 	float tweenZoomDuration = 0.1f;
	// 	float zoomShakePower = Mathf.Remap(_zoom, 0, 1, 0.05f, 0.3f);
	// 	tweenZoom.TweenProperty(Camera, "zoom", originalZoom - Vector2.One * zoomShakePower, tweenZoomDuration * 0.5f);
	// 	tweenZoom.TweenProperty(Camera, "zoom", originalZoom, tweenZoomDuration * 0.5f);

	// 	var originalPosition = Camera.GlobalPosition;
	// 	var tweenPosition = CreateTween();
	// 	float tweenPositionDuration = 0.7f;
	// 	float positionShakePower = Mathf.Remap(_zoom, 0, 1, 10, 20f);

	// 	tweenPosition.TweenProperty(Camera, "position", originalPosition + Vector2.One * positionShakePower, tweenPositionDuration * 0.3f);
	// 	tweenPosition.TweenProperty(Camera, "position", originalPosition - Vector2.One * positionShakePower, tweenPositionDuration * 0.3f);
	// 	tweenPosition.TweenProperty(Camera, "position", originalPosition, tweenPositionDuration * 0.3f);

	// 	tween.TweenSubtween(tweenZoom);
	// 	tween.TweenSubtween(tweenPosition);
	// }

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
			Camera.Position =  Camera.Position.Lerp(Camera.Position + (_dragStart - _dragEnd), delta * FollowSpeed);
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
		_zoom = Mathf.Clamp(_zoom, 0, 1);

		var newZoom = (float)Mathf.Remap(_zoom, 0, 1, 0.25, 2);

		Camera.Zoom = Camera.Zoom.Lerp(Vector2.One * newZoom, delta * ZoomSpeed);
	}
}
