using Godot;
using System;
using System.Collections.Generic;

public partial class OpponentDeck : Node2D
{
	private const string CARD_SCENE_PATH = "res://Scenes/OpponentCard.tscn";
	private const float CARD_DRAW_SPEED = 0.25f;
	private const int STARTING_HAND_SIZE = 4;

	public List<string> opponent_deck = new() { "Knight", "Fenrir", "Demon", "Mage", "Hunter", 
												"Knight", "Fenrir", "Demon", "Mage", "Hunter" };

	public override void _Ready()
	{
		Shuffle(opponent_deck);
		GetNode<RichTextLabel>("RichTextLabel").Text = opponent_deck.Count.ToString();
		
		for(int i = 0; i < STARTING_HAND_SIZE; i++){
			DrawCard();
		}
	}

	public void DrawCard()
	{	
		if (opponent_deck.Count == 0)
			return;

		string card_drawn_name = opponent_deck[0];
		opponent_deck.RemoveAt(0);

		if (opponent_deck.Count == 0)
		{
			GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Disabled = true;
			GetNode<Sprite2D>("Sprite2D").Visible = false;
			GetNode<RichTextLabel>("RichTextLabel").Visible = false;
		}

		GetNode<RichTextLabel>("RichTextLabel").Text = ""; //opponent_deck.Count.ToString();

		var card_scene = GD.Load<PackedScene>(CARD_SCENE_PATH);
		var new_card = card_scene.Instantiate<OpponentCard>();
		new_card.SetMeta("card_id", card_drawn_name);

		//string cardImagePath = $"res://Assets/Card_{card_drawn_name}.png";
		string cardImagePath = $"res://Assets/Card_Back_Opponent.png";
		Texture2D cardTexture = GD.Load<Texture2D>(cardImagePath);
		new_card.GetNode<Sprite2D>("CardImage").Texture = cardTexture;
		new_card.Attack = CardDatabase.CARDS[card_drawn_name][0];
		new_card.Name = card_drawn_name;
		new_card.GetNode<RichTextLabel>("Attack").Text = CardDatabase.CARDS[card_drawn_name][0].ToString();
		new_card.GetNode<RichTextLabel>("Health").Text = CardDatabase.CARDS[card_drawn_name][1].ToString();
		new_card.GetNode<RichTextLabel>("Attack").Visible = false;
		new_card.GetNode<RichTextLabel>("Health").Visible = false;
		GetNode("../CardManager").AddChild(new_card);

		var hand = GetNode("../OpponentHand") as OpponentHand;
		hand.AddCardToHand(new_card, CARD_DRAW_SPEED);
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
