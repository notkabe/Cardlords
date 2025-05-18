using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class BattleManager : Node
{
	private Timer battleTimer;
	private Button endTurnButton;
	private List<Node2D> emptyCardSlots = new();
	
	private const float SMALL_CARD_SCALE = 0.6f;
	private const float CARD_MOVE_SPEED = 0.2f;

	public override void _Ready()
	{
		endTurnButton = GetNode<Button>($"../EndTurnButton");
		
		// Corregido: Inicializar correctamente battleTimer
		battleTimer = GetNode<Timer>($"../BattleTimer");
		battleTimer.OneShot = true;
		battleTimer.WaitTime = 1.0;

		emptyCardSlots.Add(GetNode<Node2D>($"../CardSlots/CardSlot6"));
		emptyCardSlots.Add(GetNode<Node2D>($"../CardSlots/CardSlot7"));
		emptyCardSlots.Add(GetNode<Node2D>($"../CardSlots/CardSlot8"));
		emptyCardSlots.Add(GetNode<Node2D>($"../CardSlots/CardSlot9"));
		emptyCardSlots.Add(GetNode<Node2D>($"../CardSlots/CardSlot10"));
	}

	public void OnEndTurnButtonPressed()
	{
		_ = OpponentTurn();
	}

	private async Task OpponentTurn()
{
	endTurnButton.Visible = false;
	endTurnButton.Disabled = true;

	var deck = GetNode<OpponentDeck>("../OpponentDeck");
	if (deck.opponent_deck.Count != 0)
	{
		battleTimer.Start();
		await ToSignal(battleTimer, "timeout");

		deck.DrawCard();  // invocación directa
	}

	battleTimer.Start();
	await ToSignal(battleTimer, "timeout");

	if (emptyCardSlots.Count == 0)
	{
		EndOpponentTurn();
		return;
	}

	TryPlayCardWithHighestAttack();

	battleTimer.Start();
	await ToSignal(battleTimer, "timeout");

	EndOpponentTurn();
}

private void TryPlayCardWithHighestAttack()
{
	var opponent_hand = GetNode<OpponentHand>("../OpponentHand").opponent_hand;
	if (opponent_hand.Count == 0)
	{
		EndOpponentTurn();
		return;
	}

	// slot aleatorio
	var rng = new RandomNumberGenerator();
	rng.Randomize();
	int idx = (int)rng.RandiRange(0, (int)emptyCardSlots.Count - 1);
	var randomEmptyCardSlot = emptyCardSlots[idx];
	emptyCardSlots.RemoveAt(idx);  // mejor RemoveAt
	GD.Print($"Selecciona slot random");

	// carta con mayor ataque
	var cardWithHighestAttack = opponent_hand[0];
	foreach (var card in opponent_hand)
	{
		if(card.Attack > cardWithHighestAttack.Attack){
			cardWithHighestAttack = card;
		}
	}
	GD.Print($"Selecciona carta");
	
	string cardId = (string)cardWithHighestAttack.GetMeta("card_id");
	string cardImagePath = $"res://Assets/Card_{cardId}.png";
	Texture2D cardTexture = GD.Load<Texture2D>(cardImagePath);
	cardWithHighestAttack.GetNode<Sprite2D>("CardImage").Texture = cardTexture;
	cardWithHighestAttack.GetNode<RichTextLabel>("Attack").Visible = true;
	cardWithHighestAttack.GetNode<RichTextLabel>("Health").Visible = true;
	

	// animaciones: usa tween y tween2
	var tween = GetTree().CreateTween();
	tween.TweenProperty(cardWithHighestAttack, "position", randomEmptyCardSlot.Position, CARD_MOVE_SPEED);

	var tween2 = GetTree().CreateTween();
	tween2.TweenProperty(cardWithHighestAttack, "scale", new Vector2(SMALL_CARD_SCALE, SMALL_CARD_SCALE), CARD_MOVE_SPEED);

	// retira de la mano
	GetNode<OpponentHand>("../OpponentHand").RemoveCardFromHand(cardWithHighestAttack);
}

	private void EndOpponentTurn()
	{
		GetNode<Deck>($"../Deck").ResetDraw();
		endTurnButton.Disabled = false;
		endTurnButton.Visible = true;
	}
}
