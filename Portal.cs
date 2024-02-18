using Godot;

public partial class Portal : StaticBody2D, IDamageable
{
	private float _health = 500;
	private bool _lost;
	private ProgressBar _healthBar;

    public override void _Ready()
    {
		_healthBar = GetNode<ProgressBar>("/root/Node/Game/Camera2D/Control/PortalHealth");
    }

	void IDamageable.TakeDamage(float damage, Node2D attacker) {
		_health -= Mathf.Min(damage, _health);
		GD.Print(_health);
		_healthBar.Value = _health;
		if (_health == 0) _lost = true;
	}

	public override void _Process(double delta)
	{
		//if (!_lost) return;

		//var tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Elastic);
		//tween.TweenProperty(GetParent(), "modulate", new Color(1, 1, 1, 0), 1.0f);

	}
}
