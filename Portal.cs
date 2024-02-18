using Godot;

public partial class Portal : StaticBody2D, IDamageable
{
	private float _health;
	private bool _lost;

	void IDamageable.TakeDamage(float damage) {
		_health -= Mathf.Min(damage, _health);
		if (_health == 0) _lost = true;
	}

	public override void _Process(double delta)
	{
		if (!_lost) return;

		var tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Elastic);
		tween.TweenProperty(GetParent(), "modulate", new Color(1, 1, 1, 0), 1.0f);
	}
}
