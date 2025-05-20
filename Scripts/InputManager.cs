using Godot;
using System;

public partial class InputManager : Node2D
{
	[Signal] public delegate void LeftMouseButtonClickedEventHandler();
	[Signal] public delegate void LeftMouseButtonReleasedEventHandler();

	private const uint COLLISION_MASK_CARD = 1;
	private const uint COLLISION_MASK_DECK = 4;

	private CardManager card_manager_reference;
	private Deck deck_reference;

	// Se ejecuta al iniciar la escena, inicializa las referencias
	public override void _Ready()
	{
		card_manager_reference = GetNode<CardManager>("../CardManager");
		deck_reference = GetNode<Deck>("../Deck");
	}

	// Gestiona el input de ratón mediante Signals
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			if (mouseEvent.Pressed)
			{
				EmitSignal(SignalName.LeftMouseButtonClicked);
				RaycastAtCursor();
			}
			else
			{
				EmitSignal(SignalName.LeftMouseButtonReleased);
			}
		}
	}

	// Método general de detección de elementos bajo el cursor (No se ha llegado a implementar, se siguen
	// usando RaycastCheckForCard y RaycastCheckForCardSlot)
	public void RaycastAtCursor()
	{
		var spaceState = GetWorld2D().DirectSpaceState;
		var parameters = new PhysicsPointQueryParameters2D
		{
			Position = GetGlobalMousePosition(),
			CollideWithAreas = true
		};

		var result = spaceState.IntersectPoint(parameters);

		if (result.Count > 0)
		{
			var collider = result[0]["collider"].As<CollisionObject2D>();
			uint result_collision_mask = collider.CollisionMask;

			if (result_collision_mask == COLLISION_MASK_CARD)
			{
				var card_found = collider.GetParent<Node2D>();
				if (card_found != null && card_found is Card caCard)
				{
					card_manager_reference.CardClicked(caCard);
				}
			}
			else if (result_collision_mask == COLLISION_MASK_DECK)
			{
				deck_reference.DrawCard();
			}
		}
	}
}
