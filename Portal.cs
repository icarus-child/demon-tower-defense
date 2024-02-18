using Godot;

public partial class Portal : StaticBody2D, IDamageable
{
	private float _health = 500;
	private bool _lost;
	private ProgressBar _healthBar;

    public override void _Ready()
    {
		_healthBar = GetNode<ProgressBar>("/root/Node/Game/UI/PortalHealth");
    }

	void IDamageable.TakeDamage(float damage, Node2D attacker) {
		_health -= Mathf.Min(damage, _health);
		_healthBar.Value = _health;
		if (_health == 0) _lost = true;
	}

	public override void _Process(double delta)
	{
		if (!_lost) return;

		Game.Camera.SetProcessInput(false);
		foreach (var child in GetChildren())
		{
			child.SetProcess(false);
			child.SetProcessInput(false);
		}

		GetNode<TextureRect>("/root/Node/Game/UI/GameOver").Show();
	}
}
