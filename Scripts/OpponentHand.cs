using Godot;
using System;
using System.Collections.Generic;

public partial class OpponentHand : Node2D
{
	private const int CARD_WIDTH = 160;
	private const int HAND_Y_POSITION = 100;
	private const float DEFAULT_CARD_MOVE_SPEED= 0.1f;

	public List<OpponentCard> opponent_hand = new();
	private float center_screen_x;

	// Se ejecuta al iniciar la escena, obtiene el centro de la pantalla horizontalmente
	public override void _Ready()
	{
		center_screen_x = GetViewport().GetVisibleRect().Size.X / 2;
	}

	// Añade una carta a la mano del oponente
	public void AddCardToHand(OpponentCard card, float speed)
	{
		if (!opponent_hand.Contains(card))
		{
			opponent_hand.Insert(0, card);
			UpdateHandPositions(speed);
		}
		else
		{
			if (card.HasMeta("starting_position"))
				AnimateCardToPosition(card, (Vector2)card.GetMeta("starting_position"), DEFAULT_CARD_MOVE_SPEED);
		}
	}

	// Actualiza la posicion de las cartas en la mano para que se vean centradas
	public void UpdateHandPositions(float speed)
	{
		for (int i = 0; i < opponent_hand.Count; i++)
		{
			var new_position = new Vector2(CalculateCardPosition(i), HAND_Y_POSITION);
			var card = opponent_hand[i];
			card.SetMeta("starting_position", new_position);
			AnimateCardToPosition(card, new_position, speed);
		}
	}

	// Calcula la posición de una carta para agregarla en la mano
	public float CalculateCardPosition(int index)
	{
		float total_width = (opponent_hand.Count - 1) * CARD_WIDTH;
		float x_offset = center_screen_x + index * CARD_WIDTH - total_width / 2;
		return x_offset;
	}

	// Anima ligeramente una carta hacia una posición en concreto 
	public void AnimateCardToPosition(OpponentCard card, Vector2 new_position, float speed)
	{
		var tween = GetTree().CreateTween();
		tween.TweenProperty(card, "position", new_position, speed);
	}

	// Elimina una carta de la mano del oponente (La ha jugado)
	public void RemoveCardFromHand(OpponentCard card)
	{
		if (opponent_hand.Contains(card))
		{
			opponent_hand.Remove(card);
			UpdateHandPositions(DEFAULT_CARD_MOVE_SPEED);
		}
	}
}
