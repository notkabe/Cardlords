using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerHand : Node2D
{
	private const int CARD_WIDTH = 160;
	private const int HAND_Y_POSITION = 955;
	private const float DEFAULT_CARD_MOVE_SPEED= 0.1f;

	private List<Node2D> player_hand = new();
	private float center_screen_x;

	// Se ejecuta al inicio de la escena, obtiene el centro de la pantalla horizontalmente
	public override void _Ready()
	{
		center_screen_x = GetViewport().GetVisibleRect().Size.X / 2;
	}

	// Añade una carta a la mano del jugador desde su mazo
	public void AddCardToHand(Node2D card, float speed)
	{
		if (!player_hand.Contains(card))
		{
			player_hand.Insert(0, card);
			UpdateHandPositions(speed);
		}
		else
		{
			if (card.HasMeta("starting_position"))
				AnimateCardToPosition(card, (Vector2)card.GetMeta("starting_position"), DEFAULT_CARD_MOVE_SPEED);
		}
	}

	// Actualiza visualmente las cartas de la mano para que se vean centradas
	public void UpdateHandPositions(float speed)
	{
		for (int i = 0; i < player_hand.Count; i++)
		{
			var new_position = new Vector2(CalculateCardPosition(i), HAND_Y_POSITION);
			var card = player_hand[i];
			card.SetMeta("starting_position", new_position);
			AnimateCardToPosition(card, new_position, speed);
		}
	}

	// Calcula la posición de una carta basada en su index
	public float CalculateCardPosition(int index)
	{
		float total_width = (player_hand.Count - 1) * CARD_WIDTH;
		float x_offset = center_screen_x + index * CARD_WIDTH - total_width / 2;
		return x_offset;
	}

	// Anima ligeramente una carta hacia una posición en concreto
	public void AnimateCardToPosition(Node2D card, Vector2 new_position, float speed)
	{
		var tween = GetTree().CreateTween();
		tween.TweenProperty(card, "position", new_position, speed);
	}

	// Borra una carta de la mano del jugador (al jugarla)
	public void RemoveCardFromHand(Node2D card)
	{
		if (player_hand.Contains(card))
		{
			player_hand.Remove(card);
			UpdateHandPositions(DEFAULT_CARD_MOVE_SPEED);
		}
	}
}
