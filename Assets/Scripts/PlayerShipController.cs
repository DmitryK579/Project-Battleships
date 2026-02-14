using UnityEngine;

public class PlayerShipController : ShipController
{
	private PlayerInputActions playerInputActions;
	private Vector2 movementInput;
	private void Awake()
	{
		playerInputActions = new PlayerInputActions();
		playerInputActions.Player.Enable();
	}
	
	public override Vector2 GetMovementInput()
	{
		movementInput = playerInputActions.Player.Move.ReadValue<Vector2>();
		return movementInput;
	}
}
