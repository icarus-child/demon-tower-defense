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
	private readonly List<Node2D> targetOptions = new();
	private AnimatedSprite2D _sprite;
	
    public override void _Ready()
    {
		navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		aggroRange = GetNode<Area2D>("AggroRange");
		attackRange = GetNode<Area2D>("AttackRange");
		aggroRange.BodyEntered += body => {
			targetOptions.Add(body);
		};
		aggroRange.BodyExited += body => {
			targetOptions.Remove(body);
		};
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_sprite.Play("walk");
    }

    public override void _Process(double delta)
    {
	    if (Target == null) return;
	    _sprite.FlipH = navigationAgent.GetNextPathPosition().X > ToGlobal(Position).X; // Make sprite face the target
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

	/*public override void _Input(InputEvent @event)
	{

		if (EntityTeam == Team.Humans) return;
		if (@event is InputEventMouseButton mouse && mouse.ButtonIndex == MouseButton.Left)
			if (_sprite.HasPoint(mouse.Position))
		{
				GD.Print("aaaaaaaaaa");
			{
				_selected = true;
			else
			}
			{

				if (_selected)
				{
					GD.Print("pain");
					_selected = false;
					Target = Game.Cursor;
				}
			}
		}
	}*/

