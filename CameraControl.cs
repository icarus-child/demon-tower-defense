using Godot;

public partial class CameraControl : Camera2D
{
	[Export] private float _zoomStep = 0.25F;
	[Export] private float _maxZoom = 100;
	[Export] private Vector2 _slideThreshold = new (550, 250);

	private Vector2 _initialDragPos;

	public override void _Process(double delta)
	{	
		// Camera Slide
		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector2 dist = mousePos - GetViewportRect().Size / 2;
		if (Input.IsActionPressed("drag") || dist.Abs().X < _slideThreshold.X && dist.Abs().Y < _slideThreshold.Y) return;
		Position = Position.Lerp(dist, (float) delta);
	}

	public override void _Input(InputEvent @event)
	{
		// Camera Zoom
		if (@event.IsActionPressed("zoom_in"))
			Zoom = new Vector2(Zoom.X + _zoomStep, Zoom.Y + _zoomStep).Clamp(Vector2.One, new Vector2(_maxZoom, _maxZoom));
		if (@event.IsActionPressed("zoom_out"))
			Zoom = new Vector2(Zoom.X - _zoomStep, Zoom.Y - _zoomStep).Clamp(Vector2.One, new Vector2(_maxZoom, _maxZoom));

		// Camera drag
		Vector2 zoom = new Vector2(1 / Zoom.X, 1 / Zoom.Y);
		if (@event.IsActionPressed("drag") && @event is InputEventMouseButton mouseButton)
			_initialDragPos = Position + mouseButton.Position * zoom;
		if (Input.IsActionPressed("drag") && @event is InputEventMouseMotion mouseMotion)
			Position = _initialDragPos - mouseMotion.Position  * zoom;
	}
}
