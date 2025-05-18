using Godot;
using System;

public partial class OpponentCard : Node2D
{
	public string CardName { get; set; } = "";
	public int Attack { get; set; }
	public int Health { get; set; }
}
