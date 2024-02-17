using Godot;

public partial class Navigation : CharacterBody2D
{
	[Export]
	private Node2D target;
	[Export]
	private float speed = 35;
	private NavigationAgent2D navigationAgent;

    public override void _Ready()
    {
		navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
    }

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = ToLocal(navigationAgent.GetNextPathPosition()).Normalized();
		Velocity = direction * speed;
		GD.Print(Velocity);
		MoveAndSlide();
	}
	
	private void CreatePath()
	{
		navigationAgent.TargetPosition = target.GlobalPosition;
	}
}