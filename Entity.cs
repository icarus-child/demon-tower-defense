using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Entity : CharacterBody2D
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
	[Export] private bool _attackCooldown;
	[Export] private bool _doesAoe;
	[Export] private bool _aoeRange;
	[Export] private int _wallMultiplier;
	[Export] private int _enemyMultiplier;
	[Export] private float speed = 35;

	[Export]
	public Node2D Portal;
	public Node2D Target;
	private NavigationAgent2D navigationAgent;
	private Area2D aggroRange;
	private Area2D attackRange;
	private Area2D _mouseClicker;
	private readonly List<Node2D> targetOptions = new();
	private AnimatedSprite2D _sprite;
	public Marker2D Cursor = new ();
	private bool _selected;
	
    public override void _Ready()
    {
		navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		aggroRange = GetNode<Area2D>("AggroRange");
		attackRange = GetNode<Area2D>("AttackRange");
		_mouseClicker = GetNode<Area2D>("MouseClicker");
		aggroRange.BodyEntered += body => {
			targetOptions.Add(body);
		};
		aggroRange.BodyExited += body => {
			targetOptions.Remove(body);
		};
		_mouseClicker.InputEvent += (viewport, @event, idx) =>
		{
			if (EntityTeam == Team.Humans || !@event.IsActionPressed("select")) return;
			if (!_selected) _selected = true;
		};

		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_sprite.Play("walk");
    }

    public override void _Input(InputEvent @event)
    {
	    if (_selected && @event.IsActionPressed("select"))
	    {
		    Cursor.GlobalPosition = GetGlobalMousePosition();
		    Target = Cursor;
		    _selected = false;
	    }
    }

    public override void _Process(double delta)
    {
	    if (Target == null || navigationAgent.IsNavigationFinished()) return;

	    // Make this not shit later
	    try {
		    int i = navigationAgent.GetCurrentNavigationPathIndex() - 2;
		    _sprite.FlipH = navigationAgent.GetCurrentNavigationPath()[i].X > ToGlobal(Position).X; // Make sprite face the target
	    } catch (IndexOutOfRangeException e) {}

    }

	public override void _PhysicsProcess(double delta)
	{
		if (Target is null) return;
		if (!attackRange.OverlapsBody(Target))
		{
			AttackMove();
		}
	}

	// Attack Move implementation, need just a default Move implementation
	private void Path()
	{
		// Pathing
		if (navigationAgent.IsNavigationFinished()) return;
		Vector2 direction = ToLocal(navigationAgent.GetNextPathPosition()).Normalized();
		Velocity = direction * speed;
		MoveAndSlide();
	}

	private void AttackMove()
	{
		if ((Target == Portal || Target is null) && targetOptions.Count != 0) Target = GetClosestTargetable();
		Path();
	}
	
	private void CreatePath()
	{
		if (Target is null && Portal is null) return;

		// Target Reaquisition
		if (Target is null) {
			if (targetOptions.Count != 0) Target = GetClosestTargetable();
			else Target = Portal;
		}
		navigationAgent.TargetPosition = Target.GlobalPosition;
	}

	private Node2D GetClosestTargetable()
	{
		return targetOptions.MinBy(node =>
		{
			return node.GlobalPosition.DistanceTo(GlobalPosition);
		});
	}

	public void TakeDamage(int damage) {
		// never go below 0 hp
		_health -= Mathf.Min(damage, _health);
		if (_health == 0) Die();
		// calculate targetting attacker
	}
	
	private void Die()
	{
		if (EntityTeam == Team.Humans) Game.Souls++;
		QueueFree();
	}
}