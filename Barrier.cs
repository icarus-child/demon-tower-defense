using Godot;

public partial class Barrier : Node2D, IDamageable
{

	public float Health { get => _health; }
	[Export] private float _health = 100;
	
	void IDamageable.TakeDamage(float damage, Node2D attacker)
	{
		// never go below 0 hp
		_health -= Mathf.Min(damage, _health);
		if (_health == 0) Die();
	}
	
	private void Die()
	{
		QueueFree();
	}
}
