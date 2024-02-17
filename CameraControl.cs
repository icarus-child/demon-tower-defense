using Godot;
using System;

public partial class CameraControl : Camera2D
{
	[Export]
	private float ZoomStep;

	private Vector2 _initalDragPos;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("drag") && @event is InputEventMouseButton mouseButton)
		{
			_initalDragPos = Position + mouseButton.Position;
		}

		if (Input.IsActionPressed("drag") && @event is InputEventMouseMotion mouseMotion)
		{
			Position = (_initalDragPos - mouseMotion.Position);
		}

		if (@event.IsActionPressed("zoom_in"))
		{
			Zoom = new Vector2(Zoom.X + ZoomStep, Zoom.Y + ZoomStep);
		}

		if (@event.IsActionPressed("zoom_out"))
		{
			Zoom = new Vector2(Zoom.X - ZoomStep, Zoom.Y - ZoomStep);
		}
	}
}
