using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Entity : CharacterBody2D, IDamageable
{
	public enum Team
	{
		Humans,
		Demons
	}

	[Export] public Team EntityTeam { get; set; }
	[Export] private float _health;
	[Export] private float _damage;
	[Export] private double _attackCooldown;
	[Export] private float _wallMultiplier;
	[Export] private float _enemyMultiplier;
	[Export] private float speed;

	[Export]
	public Node2D Portal;
	public Node2D Target;
	private NavigationAgent2D _navigationAgent;
	private Area2D _aggroRange;
	private Area2D _attackRange;
	private readonly List<Node2D> _targetOptions = new();
	private Area2D _mouseClicker;
	private AnimatedSprite2D _sprite;
	public Marker2D Cursor = new ();
	private bool _selected;
	private Timer _attackCooldownTimer;
	private bool _canAttack = false;
	private bool _animationFinished = false;
	private ProgressBar _healthBar;
	private float _maxHealth;
	private Area2D _healZone;
	
    public override void _Ready()
    {
        _attackCooldownTimer = new()
        {
            OneShot = true,
            WaitTime = _attackCooldown,
			Autostart = true
        };
		AddChild(_attackCooldownTimer);
		_healthBar = GD.Load<PackedScene>("res://characters/HeathBar.tscn").Instantiate<ProgressBar>();
		AddChild(_healthBar);
		var blueBar = new StyleBoxFlat()
		{
			BgColor = new Color(0.557f, 0.745f, 0.839f, 1),
			CornerRadiusBottomRight = 7,
			CornerRadiusTopLeft = 7,
			CornerRadiusTopRight = 7,
			CornerRadiusBottomLeft = 7
		};
		var redBar = new StyleBoxFlat()
		{
			BgColor = new Color(0.929f, 0.231f, 0.267f, 1),
			CornerRadiusBottomRight = 7,
			CornerRadiusTopLeft = 7,
			CornerRadiusTopRight = 7,
			CornerRadiusBottomLeft = 7
		};
		_healthBar.AddThemeStyleboxOverride("fill", EntityTeam == Team.Humans ? blueBar : redBar);
		_navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		_aggroRange = GetNode<Area2D>("AggroRange");
		_attackRange = GetNode<Area2D>("AttackRange");
		_mouseClicker = GetNode<Area2D>("MouseClicker");
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_healZone = GetNode<Area2D>("/root/Node/Game/TileMap/Portal/HealZone");
		_aggroRange.BodyEntered += body => {
			_targetOptions.Add(body);
		};
		_aggroRange.BodyExited += body => {
			_targetOptions.Remove(body);
		};
		_attackCooldownTimer.Timeout += () => {
			_canAttack = true;
		};
		_sprite.AnimationFinished += () => {
			_sprite.Play("idle");
		};
		_mouseClicker.InputEvent += (viewport, @event, idx) =>
		{
			if (!@event.IsActionPressed("select")) return;
			if (EntityTeam == Team.Humans)
			{
				Game.Selected.Target = this;
				Game.Selected._selected = false;
				return;
			}

			if (!_selected)
			{
				Game.Selected = this;
				_selected = true;
			}
		};
		_sprite.Play("idle");
		_maxHealth = _health;

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
		_healthBar.Value = (_health / _maxHealth) * 100;
		if (Team.Demons == EntityTeam && _healZone.OverlapsBody(this))
		{
			_health += Mathf.Min((float) (2 * delta), _maxHealth - _health);
		}

		if (!IsInstanceValid(Target)) Target = null;
		if (Target is null) {
			if (_targetOptions.Count != 0) Target = GetClosestTargetable();
			else if (EntityTeam == Team.Humans) Target = Portal;
		}

	    // Make this not shit later
	    try {
		    int i = _navigationAgent.GetCurrentNavigationPathIndex() - 2;
		    _sprite.FlipH = _navigationAgent.GetCurrentNavigationPath()[i].X > ToGlobal(Position).X; // Make sprite face the target
	    } catch (IndexOutOfRangeException e) {}

    }

	public override void _PhysicsProcess(double delta)
	{
		if (Target is Marker2D && EndOfPathReached() && _targetOptions.Count == 1)
		{
			Target = GetClosestTargetable();
		} else if (Target is null) {
			return;
		}

		if (!_attackRange.OverlapsBody(Target))
		{
			if (EntityTeam == Team.Humans)
				AttackMove();
			else
				Move();
		} else {
			// Attack
			if (_canAttack) {
				_canAttack = false;
				_attackCooldownTimer.Start();
				_sprite.Play("attack");
				if (Target is IDamageable weezer) {
					weezer.TakeDamage(_damage * ((Target is Entity) ? _enemyMultiplier : _wallMultiplier), this);
				}
			}
		}
	}

	private void Path()
	{
		// Pathing
		if (EndOfPathReached()) {
			_sprite.Play("idle");
			return;
		}
		Vector2 direction = ToLocal(_navigationAgent.GetNextPathPosition()).Normalized();
		Velocity = direction * speed;
		MoveAndSlide();
		_sprite.Play("walk");
	}
	
	private bool EndOfPathReached() => _navigationAgent.GetNextPathPosition().DistanceTo(GlobalPosition) < 5;

	private void AttackMove()
	{
		if (_targetOptions.Count != 0) Target = GetClosestTargetable();
		Path();
	}
	
	private void Move()
	{
		Path();
	}

	private void CreatePath()
	{
		if (Target is null) return;
		_navigationAgent.TargetPosition = Target.GlobalPosition;
	}

	private Node2D GetClosestTargetable()
	{
		return _targetOptions.MinBy(node => {
			return node.GlobalPosition.DistanceTo(GlobalPosition);
		});
	}

	void IDamageable.TakeDamage(float damage, Node2D attacker) {
		// never go below 0 hp
		_health -= Mathf.Min(damage, _health);
		if (_health == 0) Die();
		// calculate targetting attacker
		if ((Target == Portal || Target is Marker2D) && EntityTeam is Team.Humans) {
			Target = attacker;
		}
	}
	
	private void Die()
	{
		if (EntityTeam == Team.Humans) Game.AddSoul();
		QueueFree();
	}

}