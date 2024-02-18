using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Enemy : CharacterBody2D
{
	public enum Team
	{
		Humans,
		Demons
	}

	[Export] public Team EntityTeam { get; set; }
	[Export] private int _health;
	[Export] private int _damage;
	[Export] private bool _attackRange;
	[Export] private bool _doesAoe;
	[Export] private bool _aoeRange;
	[Export] private int _wallMultiplier;
	[Export] private int _enemyMultiplier;
	[Export] private float speed = 35;

	[Export]
	public Node2D Portal;
	public Node2D Target = null;
	public int TargetingPriority { get => _targetingPriority; }
	private int _targetingPriority = -1;
	private NavigationAgent2D navigationAgent;
	private Area2D aggroRange;
	private List<Node2D> targetBacklog = new();
	
    public override void _Ready()
    {
		navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		aggroRange = GetNode<Area2D>("AggroRange");
		aggroRange.BodyEntered += body => {
			if (Target == Portal || Target is null) Target = body;
			else targetBacklog.Add(body);
		};
    }

	public override void _PhysicsProcess(double delta)
	{
		// Pathing
		//if (navigationAgent.IsNavigationFinished()) return;
		Vector2 direction = ToLocal(navigationAgent.GetNextPathPosition()).Normalized();
		Velocity = direction * speed;
		MoveAndSlide();
	}
	
	private void CreatePath()
	{	
		// Target Reaquisition
		if (Target is null) {
			GD.Print("Hit");
			if (targetBacklog.Count != 0) Target = GetClosestTargetable();
			else Target = Portal;
		}
		navigationAgent.TargetPosition = Target.GlobalPosition;
	}

	private Node2D GetClosestTargetable()
	{
		return targetBacklog.MinBy(node => {
			return node.GlobalPosition.DistanceTo(GlobalPosition);
		});
	}

	public void TakeDamage(int damage) {
		// never go below 0 hp
		_health -= Mathf.Min(damage, _health);
		if (_health == 0) Die();
	}
	
	private void Die()
	{
		QueueFree();
	}
		// calculate targetting attacker
}