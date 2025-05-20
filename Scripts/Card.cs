using Godot;
using System;

public partial class Card : Node2D
{

	[Signal]
	public delegate void HoveredEventHandler(Card card);

	[Signal]
	public delegate void HoveredOffEventHandler(Card card);
	
	public string card_type;
	public int Attack { get; set; }
	public int Health { get; set; }
	public CardSlot CardSlotIsIn { get; set; }

 // Se llama al cargar la escena por primera vez
	public override void _Ready()
	{
		base._Ready();

		if (GetParent() is CardManager manager)
		{
			manager.ConnectCardSignals(this);
		}
		else
		{
			GD.PrintErr($"El padre de {Name} no es CardManager, es {GetParent()?.GetType().Name}");
		}
	}

// Se llama cuando el ratón entra en el área de la carta (Area2D)
	public void OnArea2DMouseEntered()
	{
		EmitSignal(SignalName.Hovered, this);
	}

  // Se llama cuando el ratón sale del área de la carta (Area2D)
	public void OnArea2DMouseExited()
	{
		EmitSignal(SignalName.HoveredOff, this);
	}
	
	public void ClearSlot()
	{
		if (CardSlotIsIn != null)
		{
			CardSlotIsIn.cardInSlot = false;
			CardSlotIsIn = null;
		}
	}
}
