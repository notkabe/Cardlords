using Godot;
using System;
using System.Collections.Generic;

public partial class Deck : Node2D
{
	private const string CARD_SCENE_PATH = "res://Scenes/Card.tscn";
	private const float CARD_DRAW_SPEED = 0.25f;
	private const int STARTING_HAND_SIZE = 4;

	private List<string> player_deck = new() { "Knight", "Fenrir", "Demon", "Mage", "Hunter", 
												"Knight", "Fenrir", "Demon", "Mage", "Hunter" };
	private bool card_drawn_this_turn = false;

	public override void _Ready()
	{
		Shuffle(player_deck);
		GetNode<RichTextLabel>("RichTextLabel").Text = player_deck.Count.ToString();
		
		for(int i = 0; i < STARTING_HAND_SIZE; i++){
			DrawCard();
			card_drawn_this_turn = false;
		}
	}

	public void DrawCard()
	{
		if(card_drawn_this_turn){
			return;
		}
		
		card_drawn_this_turn = true; 
		
		if (player_deck.Count == 0)
			return;

		string card_drawn_name = player_deck[0];
		player_deck.RemoveAt(0);

		if (player_deck.Count == 0)
		{
			GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Disabled = true;
			GetNode<Sprite2D>("Sprite2D").Visible = false;
			GetNode<RichTextLabel>("RichTextLabel").Visible = false;
		}

		GetNode<RichTextLabel>("RichTextLabel").Text = player_deck.Count.ToString();

		var card_scene = GD.Load<PackedScene>(CARD_SCENE_PATH);
		Card new_card = card_scene.Instantiate<Card>();
		string cardImagePath = $"res://Assets/Card_{card_drawn_name}.png";
		Texture2D cardTexture = GD.Load<Texture2D>(cardImagePath);
		new_card.GetNode<Sprite2D>("CardImage").Texture = cardTexture;
		new_card.Name = "card_drawn_name";
		new_card.Attack = CardDatabase.CARDS[card_drawn_name][0];
		new_card.Health = CardDatabase.CARDS[card_drawn_name][1];
		new_card.GetNode<RichTextLabel>("Attack").Text = new_card.Attack.ToString();
		new_card.GetNode<RichTextLabel>("Health").Text = new_card.Health.ToString();
		GetNode("../CardManager").AddChild(new_card);

		var hand = GetNode("../PlayerHand") as PlayerHand;
		hand.AddCardToHand(new_card, CARD_DRAW_SPEED);
	}
	
	public void ResetDraw(){
		card_drawn_this_turn = false;
	}
	
	public static void Shuffle<T>(List<T> list)
	{
		Random rng = new Random();
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			(list[n], list[k]) = (list[k], list[n]);
		}
	}
}
