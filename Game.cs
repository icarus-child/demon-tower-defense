using Godot;
using System;
using System.Collections.Generic;
using DemonTowerDefense;

public partial class Game : Node2D
{
	private readonly Random _rng = new ();
	public static int Souls;
	private readonly Dictionary<PackedScene, float> _humans = new ();
	private readonly Dictionary<PackedScene, float> _demons = new ();

	private Marker2D _portal;
	public static Marker2D Cursor;

	private void RegisterHuman(String scene, float spawnChance)
	{
		_humans.Add(GD.Load<PackedScene>(scene), spawnChance);
	}

	private void RegisterDemon(String scene, float spawnChance)
	{
		_demons.Add(GD.Load<PackedScene>(scene), spawnChance);
	}

	public override void _Ready()
	{
		_portal = GetNode<Marker2D>("TileMap/Target");
		Cursor = GetNode<Marker2D>("TileMap/Cursor");

		RegisterDemon("res://characters/demons/TestDemon.tscn", 0.5f);

		RegisterHuman("res://characters/humans/TestHuman.tscn", 0.5f);
	}

	private void SpawnDemon()
	{
		Entity demon = _demons.RandomElementByWeight(e => e.Value).Key.Instantiate<Entity>();
		_portal.AddChild(demon);
	}

	private void SpawnHuman()
	{
		Entity human = _humans.RandomElementByWeight(e => e.Value).Key.Instantiate<Entity>();
		human.Portal = _portal;
		Node node = GetNode("TileMap/Spawn");
		node.GetNode<Marker2D>(_rng.Next(1, node.GetChildCount() + 1).ToString()).AddChild(human);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouse && mouse.ButtonIndex == MouseButton.Left)
		{
			Cursor.Position = mouse.Position;
		}
	}
}
