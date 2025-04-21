using Godot;
using System;

public partial class CardManager : Node2D
{
	private const uint COLLISION_MASK_CARD = 1;

	private Vector2 screenSize;
	private Node2D cardBeingDragged = null;
	private bool isHoveringOnCard = false;

	public override void _Ready()
	{
		screenSize = GetViewportRect().Size;
	}

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

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			if (mouseEvent.Pressed)
			{
				GD.Print("Left Click");
				Node2D card = RaycastCheckForCard();
				if (card != null)
				{
					StartDrag(card);
				}
			}
			else
			{
				FinishDrag();
			}
		}
	}
	
	 public void ConnectCardSignals(Node2D card)
	{
		card.Connect("hovered", new Callable(this, nameof(OnHoveredOverCard)));
		card.Connect("hovered_off", new Callable(this, nameof(OnHoveredOffCard)));
	}
	
	private void OnHoveredOverCard(Node2D card)
	{
		if (!isHoveringOnCard && cardBeingDragged == null)
		{
			isHoveringOnCard = true;
			HighlightCard(card, true);
		}
	}

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

	private void HighlightCard(Node2D card, bool hovered)
	{
		if (hovered)
		{
			card.Scale = new Vector2(1.05f, 1.05f);
			card.ZIndex = 2;
		}
		else
		{
			card.Scale = new Vector2(1f, 1f);
			card.ZIndex = 1;
		}
	}

	public void StartDrag(Node2D card)
	{
		cardBeingDragged = card;
		card.Scale = new Vector2(1f, 1f);
	}

	public void FinishDrag()
	{
		if (cardBeingDragged != null)
		{
			cardBeingDragged.Scale = new Vector2(1.05f, 1.05f);
			cardBeingDragged = null;
		}
	}

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
			return GetCardWithHighestZIndex(result);
		}

		return null;
	}

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
