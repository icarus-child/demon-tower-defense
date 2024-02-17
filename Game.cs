using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using DemonTowerDefense;

public partial class Game : Node2D
{
	private readonly Random _rng = new ();
	private readonly Dictionary<PackedScene, float> _humans = new ();

	private void RegisterHuman(String scene, float spawnChance)
	{
		_humans.Add(GD.Load<PackedScene>(scene), spawnChance);
	}

	public override void _Ready()
	{
		RegisterHuman("res://characters/humans/TestHuman.tscn", 0.5f);
		//RegisterHuman("res://characters/Humans/...", 0.5f);
		//RegisterHuman("res://characters/Humans/...", 0.5f);
		//RegisterHuman("res://characters/Humans/...", 0.5f);
	}

	private void SpawnHuman()
	{
		Node human = _humans.RandomElementByWeight(e => e.Value).Key.Instantiate();
		
		GetNode<Marker2D>("TileMap/Spawn/" + _rng.Next(1, 4)).AddChild(human);
	}
}
