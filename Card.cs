using Godot;
using System;

public partial class Card : Node2D
{
	
	[Signal]
	public delegate void HoveredEventHandler(Card card);

	[Signal]
	public delegate void HoveredOffEventHandler(Card card);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetParent().Call("connect_card_signals", this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void OnArea2DMouseEntered(){
		 EmitSignal(SignalName.Hovered, this);
	}
	
	public void OnArea2DMouseExited(){
		 EmitSignal(SignalName.HoveredOff, this);
	}
}
