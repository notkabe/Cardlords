using Godot;
using System;
using System.Collections.Generic;

public partial class CardManager : Node2D
{
	private const uint COLLISION_MASK_CARD = 1;
	private const uint COLLISION_MASK_CARD_SLOT = 2;
	private const float DEFAULT_CARD_MOVE_SPEED = 0.1f;
	private const float DEFAULT_CARD_SCALE = 0.6f;
	private const float BIGGER_CARD_SCALE = 0.65f;

	private Vector2 screenSize;
	private Node2D cardBeingDragged = null;
	private bool isHoveringOnCard = false;
	private bool playedCardThisTurn = false;
	private List<CardSlot> invalidSlots;
	private Card selectedCard;
	 
	
	private PlayerHand player_hand_reference;

 // Se ejecuta al entrar en la escena
	public override void _Ready()
	{
		screenSize = GetViewportRect().Size;
		player_hand_reference = GetNode<PlayerHand>("../PlayerHand");
		GetNode<InputManager>("../InputManager").Connect("LeftMouseButtonReleased", new Callable(this, nameof(OnLeftClickReleased)));
		
		invalidSlots = GetNode<BattleManager>($"../BattleManager").emptyEnemyCardSlots;
	}

 //Se llama cada frame
	public override void _Process(double delta)
	{
		if (cardBeingDragged != null)
		{
			Vector2 mousePos = GetGlobalMousePosition();
			cardBeingDragged.Position = new Vector2(
				Mathf.Clamp(mousePos.X, 0, screenSize.X),
				Mathf.Clamp(mousePos.Y, 0, screenSize.Y)
			);
		}
	}
	
	/* Conecta las señales de hover de la carta al gestor:
	   - "Hovered"     → OnHoveredOverCard
	   - "HoveredOff"  → OnHoveredOffCard
	*/
	 public void ConnectCardSignals(Node2D card)
	{
		card.Connect("Hovered", new Callable(this, nameof(OnHoveredOverCard)));
		card.Connect("HoveredOff", new Callable(this, nameof(OnHoveredOffCard)));
	}
	
	public void OnLeftClickReleased(){
		if(cardBeingDragged != null){
			FinishDrag();
		}
	}
	
	// Se llama cuando el cursor pasa por encima de una carta
	private void OnHoveredOverCard(Node2D card)
	{
		if(card is Card cCard && cCard.CardSlotIsIn != null){
			return;
		}
		
		if (!isHoveringOnCard && cardBeingDragged == null)
		{
			isHoveringOnCard = true;
			HighlightCard(card, true);
		}
		
	}

// Se llama cuando el cursor sale de una carta
	private void OnHoveredOffCard(Node2D card)
	{
		if (cardBeingDragged == null)
		{
			HighlightCard(card, false);

			var newCardHovered = RaycastCheckForCard();
			if (newCardHovered != null)
			{
				HighlightCard(newCardHovered, true);
			}
			else
			{
				isHoveringOnCard = false;
			}
		}
	}

 // Cambia visualmente la carta para resaltar o quitar el resaltado
	private void HighlightCard(Node2D card, bool hovered)
	{
		if (hovered)
		{
			card.Scale = new Vector2(BIGGER_CARD_SCALE, BIGGER_CARD_SCALE);
			card.ZIndex = 2;
		}
		else
		{
			card.Scale = new Vector2(DEFAULT_CARD_SCALE, DEFAULT_CARD_SCALE);
			card.ZIndex = 1;
		}
	}
	
	public void CardClicked(Card card){
		if (card is Card playerCard){
			if (playerCard.CardSlotIsIn != null){
				var battleManager = GetNode<BattleManager>("../BattleManager");
				
				if (!battleManager.cardsAttackedThisTurn.Contains(playerCard)){
					if (battleManager.opponentCardsOnBattlefield.Count == 0){
						battleManager.DirectAttack(playerCard, "Player");
						return;
					}
					else {
						SelectCardForBattle(card);
					}
				}
			} else {
				StartDrag(card);
			}
		}
	}

	
	private void SelectCardForBattle(Card card){
		if(selectedCard != null){
			if(selectedCard == card){
				//card.Position = new Vector2(card.Position.X, card.Position.Y + 20);
				selectedCard = null;
			}else{
				//selectedCard.Position = new Vector2(selectedCard.Position.X, selectedCard.Position.Y + 20);
				selectedCard = card;
				//card.Position  = new Vector2(card.Position.X, card.Position.Y - 20);
			}
		}else{
			selectedCard = card;
			//card.Position  = new Vector2(card.Position.X, card.Position.Y - 20);;
		}
	}

// Inicia el arrastre de la carta indicada
	public void StartDrag(Node2D card)
	{
		cardBeingDragged = card;
		card.Scale = new Vector2(BIGGER_CARD_SCALE, BIGGER_CARD_SCALE);
	}

	// Finaliza el arrastre y restaura la escala original
	public void FinishDrag()
	{
		
		if (cardBeingDragged != null)
		{
			var cardSlotFound = RaycastCheckForCardSlot();

			if (cardSlotFound != null && !cardSlotFound.cardInSlot && !invalidSlots.Contains(cardSlotFound))
			{
				cardBeingDragged.Position = cardSlotFound.Position;

				cardSlotFound.cardInSlot = true;
				cardSlotFound.GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Disabled = true;

				if(cardBeingDragged is Card card){
					card.CardSlotIsIn = cardSlotFound;
				}

				player_hand_reference.RemoveCardFromHand(cardBeingDragged);

				if (cardBeingDragged is Card cbdCard)
				{
					GetNode<BattleManager>($"../BattleManager").playerCardsOnBattlefield.Add(cbdCard);
				}
			}
			else
			{
				player_hand_reference.AddCardToHand(cardBeingDragged, DEFAULT_CARD_MOVE_SPEED);
			}

			cardBeingDragged.Scale = new Vector2(DEFAULT_CARD_SCALE, DEFAULT_CARD_SCALE);
			cardBeingDragged = null;
		}
	}

	// Devuelve la carta bajo el cursor usando un punto de colisión
	 private Node2D RaycastCheckForCard()
	{
		var spaceState = GetWorld2D().DirectSpaceState;
		var parameters = new PhysicsPointQueryParameters2D
		{
			Position = GetGlobalMousePosition(),
			CollideWithAreas = true,
			CollisionMask = COLLISION_MASK_CARD
		};

		var result = spaceState.IntersectPoint(parameters);
		if (result.Count > 0)
		{
			GD.Print(GetCardWithHighestZIndex(result));
			return GetCardWithHighestZIndex(result);
		}

		return null;
	}
	
	private CardSlot RaycastCheckForCardSlot()
	{
		var spaceState = GetWorld2D().DirectSpaceState;
		var parameters = new PhysicsPointQueryParameters2D
		{
			Position = GetGlobalMousePosition(),
			CollideWithAreas = true,
			CollisionMask = COLLISION_MASK_CARD_SLOT
		};

		var result = spaceState.IntersectPoint(parameters);
		if (result.Count > 0)
		{
			return result[0]["collider"].As<Node2D>().GetParent<CardSlot>();
		}

		return null;
	}

// Selecciona la carta con mayor ZIndex de entre las colisiones detectadas
	private Node2D GetCardWithHighestZIndex(Godot.Collections.Array<Godot.Collections.Dictionary> cards)
	{
	var highestZCard = cards[0]["collider"].As<Node2D>().GetParent<Node2D>();
	int highestZIndex = highestZCard.ZIndex;

	for (int i = 1; i < cards.Count; i++)
	{
		var currentCard = cards[i]["collider"].As<Node2D>().GetParent<Node2D>();
		int currentZ = currentCard.ZIndex;

		if (currentZ > highestZIndex)
		{
			highestZCard = currentCard;
			highestZIndex = currentZ;
		}
	}

	return highestZCard;
	}
}
