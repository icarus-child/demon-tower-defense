using Godot;

public partial class Entity : CharacterBody2D
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
}
