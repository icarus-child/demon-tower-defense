using Godot;
using System;
using System.Collections.Generic;
using DemonTowerDefense;

public partial class Game : Node2D
{
	private readonly Random _rng = new ();
	private readonly Dictionary<PackedScene, float> _humans = new ();

	private Marker2D _target;

	private void RegisterHuman(String scene, float spawnChance)
	{
		_humans.Add(GD.Load<PackedScene>(scene), spawnChance);
	}

	public override void _Ready()
	{
		_target = GetNode<Marker2D>("TileMap/Target");

		RegisterHuman("res://characters/humans/TestHuman.tscn", 0.5f);
		//RegisterHuman("res://characters/Humans/...", 0.5f);
		//RegisterHuman("res://characters/Humans/...", 0.5f);
		//RegisterHuman("res://characters/Humans/...", 0.5f);
	}

	private void SpawnHuman()
	{
		Navigation human = _humans.RandomElementByWeight(e => e.Value).Key.Instantiate<Navigation>();
		human.Target = _target;
		Node node = GetNode("TileMap/Spawn");
		node.GetNode<Marker2D>(_rng.Next(1, node.GetChildCount()).ToString()).AddChild(human);
	}
}
