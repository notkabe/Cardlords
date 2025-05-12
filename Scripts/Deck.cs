using Godot;
using System;
using System.Collections.Generic;

public partial class Deck : Node2D
{
	private const string CARD_SCENE_PATH = "res://Scenes/Card.tscn";
	private const float CARD_DRAW_SPEED = 1f;

	private List<string> player_deck = new() { "Tortuig", "Tortuig", "Tortuig" };

	public override void _Ready()
	{
		GetNode<RichTextLabel>("RichTextLabel").Text = player_deck.Count.ToString();
	}

	public void DrawCard()
	{
		if (player_deck.Count == 0)
			return;

		string card_drawn = player_deck[0];
		player_deck.RemoveAt(0);

		if (player_deck.Count == 0)
		{
			GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Disabled = true;
			GetNode<Sprite2D>("Sprite2D").Visible = false;
			GetNode<RichTextLabel>("RichTextLabel").Visible = false;
		}

		GetNode<RichTextLabel>("RichTextLabel").Text = player_deck.Count.ToString();

		var card_scene = GD.Load<PackedScene>(CARD_SCENE_PATH);
		var new_card = card_scene.Instantiate<Node2D>();
		GetNode("../CardManager").AddChild(new_card);
		new_card.Name = "Card";

		var hand = GetNode("../PlayerHand") as PlayerHand;
		hand.AddCardToHand(new_card, CARD_DRAW_SPEED);
	}
}
