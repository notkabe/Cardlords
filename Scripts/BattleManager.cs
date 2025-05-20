using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class BattleManager : Node
{
	private Timer battleTimer;
	private Button endTurnButton;
	private RandomNumberGenerator rng;
	public List<CardSlot> emptyEnemyCardSlots = new();
	public List<OpponentCard> opponentCardsOnBattlefield = new();
	public List<Card> playerCardsOnBattlefield = new();
	public List<Card> cardsAttackedThisTurn = new ();
	
	private const float SMALL_CARD_SCALE = 0.6f;
	private const float CARD_MOVE_SPEED = 0.2f;
	private const int STARTING_HEALTH = 15;
	private const int BATTLE_POS_OFFSET = 25;
	
	private int player_health;
	private int opponent_health;
	private bool cardWasDestroyed;
	private int zindexValue;
	
	private void DebugMsg(string message){
		GD.Print(message);
	}

	public override void _Ready()
	{
		endTurnButton = GetNode<Button>($"../EndTurnButton");
		
		battleTimer = GetNode<Timer>($"../BattleTimer");
		battleTimer.OneShot = true;
		battleTimer.WaitTime = 1.0;
		
		rng = new RandomNumberGenerator();

		emptyEnemyCardSlots.Add(GetNode<CardSlot>($"../CardSlots/CardSlot6"));
		emptyEnemyCardSlots.Add(GetNode<CardSlot>($"../CardSlots/CardSlot7"));
		emptyEnemyCardSlots.Add(GetNode<CardSlot>($"../CardSlots/CardSlot8"));
		emptyEnemyCardSlots.Add(GetNode<CardSlot>($"../CardSlots/CardSlot9"));
		emptyEnemyCardSlots.Add(GetNode<CardSlot>($"../CardSlots/CardSlot10"));
		
		player_health = STARTING_HEALTH;
		GetNode<RichTextLabel>($"../PlayerHealth").Text = player_health.ToString();
		opponent_health = STARTING_HEALTH;
		GetNode<RichTextLabel>($"../OpponentHealth").Text = opponent_health.ToString();
		
		cardWasDestroyed = false;
		zindexValue = 1;
	}

	public void OnEndTurnButtonPressed()
	{
		cardsAttackedThisTurn = new ();
		OpponentTurn();
	}

	private async Task OpponentTurn()
	{
		endTurnButton.Visible = false;
		endTurnButton.Disabled = true;

		OpponentDeck deck = GetNode<OpponentDeck>("../OpponentDeck");
		if (deck.opponent_deck.Count != 0)
		{
			await Wait(1f);
			deck.DrawCard();
		}

		await Wait(1f);

		// Mira si hay un cardslot libre y juega la carta con el mayor ataque
		if (emptyEnemyCardSlots.Count != 0)
		{
			TryPlayCardWithHighestAttack();
			await Wait(1f);
		}
		
		// Intenta atacar
		// Si hay cartas enemigas en el battlefield
		if (opponentCardsOnBattlefield.Count != 0)
		{
			rng.Randomize();

			// Hacemos una copia para iterar de forma segura
			List<OpponentCard> enemyCardsToAttack = new List<OpponentCard>(opponentCardsOnBattlefield);

			foreach (OpponentCard card in enemyCardsToAttack)
			{
				// Filtrar solo cartas vivas
				var validTargets = playerCardsOnBattlefield.FindAll(c => c.Health > 0);

				if (validTargets.Count > 0)
				{
					var cardToAttack = validTargets[rng.RandiRange(0, validTargets.Count - 1)];
					await PerformAttack(card, cardToAttack, "Opponent");
				}
				else
				{
					await DirectAttack(card, "Opponent");
				}
			}
		}
		
		await Wait(1f);
		EndOpponentTurn();
	}

	private void TryPlayCardWithHighestAttack()
	{
		List<OpponentCard> opponent_hand = GetNode<OpponentHand>("../OpponentHand").opponent_hand;
		if (opponent_hand.Count == 0)
		{
			EndOpponentTurn();
			return;
		}

		// slot aleatorio
		rng.Randomize();
		int idx = (int)rng.RandiRange(0, (int)emptyEnemyCardSlots.Count - 1);
		CardSlot randomEmptyCardSlot = emptyEnemyCardSlots[idx];
		emptyEnemyCardSlots.RemoveAt(idx);

		// carta con mayor ataque
		var cardWithHighestAttack = opponent_hand[0];
		foreach (OpponentCard card in opponent_hand)
		{
			if(card.Attack > cardWithHighestAttack.Attack){
				cardWithHighestAttack = card;
			}
		}
		
		string cardId = (string)cardWithHighestAttack.GetMeta("card_id");
		string cardImagePath = $"res://Assets/Card_{cardId}.png";
		Texture2D cardTexture = GD.Load<Texture2D>(cardImagePath);
		cardWithHighestAttack.GetNode<Sprite2D>("CardImage").Texture = cardTexture;
		cardWithHighestAttack.GetNode<RichTextLabel>("Attack").Visible = true;
		cardWithHighestAttack.GetNode<RichTextLabel>("Health").Visible = true;
		

		// Animaciones
		var tween = GetTree().CreateTween();
		tween.TweenProperty(cardWithHighestAttack, "position", randomEmptyCardSlot.Position, CARD_MOVE_SPEED);

		var tween2 = GetTree().CreateTween();
		tween2.TweenProperty(cardWithHighestAttack, "scale", new Vector2(SMALL_CARD_SCALE, SMALL_CARD_SCALE), CARD_MOVE_SPEED);

		// Retira de la mano
		GetNode<OpponentHand>("../OpponentHand").RemoveCardFromHand(cardWithHighestAttack);
		
		// Guarda el CardSlot
		cardWithHighestAttack.CardSlotIsIn = randomEmptyCardSlot;
		
		// Añade a lista de cartas en juego (oponente)
		opponentCardsOnBattlefield.Add(cardWithHighestAttack);
	}

	private void EndOpponentTurn()
	{
		GetNode<Deck>($"../Deck").ResetDraw();
		endTurnButton.Disabled = false;
		endTurnButton.Visible = true;
	}
	
	public async Task DirectAttack(Node2D attackingCard, string attacker)
	{
		float newPosY;
		if (attacker == "Opponent")
		{
			newPosY = 1080;
		}
		else
		{
			newPosY = 0;
		}

		var new_pos = new Vector2(attackingCard.Position.X, newPosY);
		attackingCard.ZIndex = 5;

		var tween = GetTree().CreateTween();
		tween.TweenProperty(attackingCard, "position", new_pos, CARD_MOVE_SPEED);
		await Wait(0.15f);

		if (attacker == "Opponent" && attackingCard is OpponentCard opCard)
		{
			player_health = Math.Max( 0, player_health - opCard.Attack);
			GetNode<RichTextLabel>($"../PlayerHealth").Text = player_health.ToString();
			
			var tween2 = GetTree().CreateTween();
			tween2.TweenProperty(attackingCard, "position", ((OpponentCard)attackingCard).CardSlotIsIn.Position, CARD_MOVE_SPEED);
		}
		else if(attacker == "Player" && attackingCard is Card caCard)
		{
			opponent_health = Math.Max( 0, opponent_health - caCard.Attack);
			GetNode<RichTextLabel>($"../OpponentHealth").Text = opponent_health.ToString();
			cardsAttackedThisTurn.Add(caCard);
			
			var tween3 = GetTree().CreateTween();
			tween3.TweenProperty(attackingCard, "position", ((Card)attackingCard).CardSlotIsIn.Position, CARD_MOVE_SPEED);
		}else{
			GD.Print("Nose");
		}

		attackingCard.ZIndex = 0;
		await Wait(1f);
	}

	private async Task PerformAttack(Node2D attackingCard, Node2D defendingCard, string attacker)
	{
		attackingCard.ZIndex = 5;
		var new_pos = new Vector2(defendingCard.Position.X, defendingCard.Position.Y + BATTLE_POS_OFFSET);

		var tween = GetTree().CreateTween();
		tween.TweenProperty(attackingCard, "position", new_pos, CARD_MOVE_SPEED);
		await Wait(0.15f);

		var tween2 = GetTree().CreateTween();
		tween2.TweenProperty(attackingCard, "position", GetCardSlotPosition(attackingCard), CARD_MOVE_SPEED);

		if (attacker == "Player")
		{
			Card playerCard = (Card)attackingCard;
			OpponentCard opponentCard = (OpponentCard)defendingCard;
			cardsAttackedThisTurn.Add(playerCard);

			opponentCard.Health = Math.Max(0, opponentCard.Health - playerCard.Attack);
			opponentCard.GetNode<RichTextLabel>("Health").Text = opponentCard.Health.ToString();

			if (playerCard.Health == 0)
			{
				cardWasDestroyed = true;
				await DestroyCard(playerCard, "Player");
			}

			if (opponentCard.Health == 0)
			{
				cardWasDestroyed = true;
				await DestroyCard(opponentCard, "Opponent");
			}
		}
		else
		{
			OpponentCard oppCard = (OpponentCard)attackingCard;
			Card plCard = (Card)defendingCard;

			plCard.Health = Math.Max(0, plCard.Health - oppCard.Attack);
			plCard.GetNode<RichTextLabel>("Health").Text = plCard.Health.ToString();

			if (oppCard.Health == 0)
			{
				cardWasDestroyed = true;
				await DestroyCard(oppCard, "Opponent");
			}

			if (plCard.Health == 0)
			{
				cardWasDestroyed = true;
				await DestroyCard(plCard, "Player");
			}
		}

		await Wait(0.5f);
		attackingCard.ZIndex = 0;

		if (cardWasDestroyed)
			await Wait(1f);
	}

	private Vector2 GetCardSlotPosition(Node2D card)
	{
		if (card is Card playerCard && playerCard.CardSlotIsIn != null)
			return playerCard.CardSlotIsIn.Position;

		if (card is OpponentCard opponentCard && opponentCard.CardSlotIsIn != null)
			return opponentCard.CardSlotIsIn.Position;

		return card.Position;
	}
	
	private async Task DestroyCard(Node2D card, string cardOwner)
	{
		var discardPos = cardOwner == "Player"
			? GetNode<Node2D>($"../PlayerDiscard").Position
			: GetNode<Node2D>($"../OpponentDiscard").Position;

		if (cardOwner == "Player" && card is Card plCard)
		{
			if(playerCardsOnBattlefield.Contains(plCard)){
				playerCardsOnBattlefield.Remove(plCard);
			}
		}
		else if (cardOwner == "Opponent" && card is OpponentCard opCard)
		{
			if(opponentCardsOnBattlefield.Contains(opCard)){
				opponentCardsOnBattlefield.Remove(opCard);
			}
		}
		zindexValue += 1;
		card.ZIndex = zindexValue;

		var tween = GetTree().CreateTween();
		tween.TweenProperty(card, "position", discardPos, CARD_MOVE_SPEED);
		
		if (card is Card pCard)
		{
			pCard.ClearSlot();
		}
		else if (card is OpponentCard oCard)
		{
			oCard.ClearSlot();
		}
			await Wait(0.3f);
	}

	private async Task Wait(float time)
	{
		battleTimer.WaitTime = time;
		battleTimer.Start();
		await ToSignal(battleTimer, "timeout");
	}
}
