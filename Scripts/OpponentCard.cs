using Godot;
using System;

public partial class OpponentCard : Node2D
{
	public string CardName { get; set; } = "";
	public int Attack { get; set; }
	public int Health { get; set; }
	public CardSlot CardSlotIsIn { get; set; }
	
	// Limpia el slot en el que estaba para que pueda recibir otra carta
	public void ClearSlot()
	{
		if (CardSlotIsIn != null)
		{
			CardSlotIsIn.cardInSlot = false;
			CardSlotIsIn = null;
		}
	}
}
