using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class BattleManager : Node
{
	private Timer battleTimer;
	private Button endTurnButton;
	private List<Node2D> emptyCardSlots = new();

	public override void _Ready()
	{
		endTurnButton = GetNode<Button>("EndTurnButton");

		// Corregido: Inicializar correctamente battleTimer
		battleTimer = GetNode<Timer>("BattleTimer");
		battleTimer.OneShot = true;
		battleTimer.WaitTime = 1.0;

		emptyCardSlots.Add(GetNode<Node2D>("emptyCardSlots/CardSlot1"));
		emptyCardSlots.Add(GetNode<Node2D>("emptyCardSlots/CardSlot2"));
		emptyCardSlots.Add(GetNode<Node2D>("emptyCardSlots/CardSlot3"));
		emptyCardSlots.Add(GetNode<Node2D>("emptyCardSlots/CardSlot4"));
		emptyCardSlots.Add(GetNode<Node2D>("emptyCardSlots/CardSlot5"));
	}

	public void OnEndTurnButtonPressed()
	{
		_ = OpponentTurn(); // Llamar a método async sin bloquear
	}

	private async Task OpponentTurn()
	{
		endTurnButton.Disabled = true;
		endTurnButton.Visible = false;

		GetNode<Node2D>("OpponentDeck").Call("DrawCard");

		battleTimer.Start();
		await ToSignal(battleTimer, "timeout");

		// Check if free monster card slots, if not, end turn
		if (emptyCardSlots.Count == 0)
		{
			EndOpponentTurn();
			return;
		}

		// Aquí iría la lógica de jugar la carta con más ataque...

		// End Turn
		EndOpponentTurn();
	}

	private void EndOpponentTurn()
	{
		endTurnButton.Disabled = false;
		endTurnButton.Visible = true;
	}
}
