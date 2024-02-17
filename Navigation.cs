using Godot;

public partial class Navigation : CharacterBody2D
{
	public enum Side
	{
		Humans,
		Demons
	}

	[Export] private Side _side { get; set; }
	[Export] private int _health;
	[Export] private int _damage;
	[Export] private bool _doesAoe;
	[Export] private int _wallMultiplier;
	[Export] private int _enemyMultiplier;

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
		if (navigationAgent.IsNavigationFinished()) return;
		Vector2 direction = ToLocal(navigationAgent.GetNextPathPosition()).Normalized();
		Velocity = direction * speed;
		MoveAndSlide();
	}
	
	private void CreatePath()
	{
		navigationAgent.TargetPosition = target.GlobalPosition;
	}
}