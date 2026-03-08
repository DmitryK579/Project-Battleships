using UnityEngine;

public class PlayerShipController : ShipController
{
	private Vector2 movementInput;

	private void Start()
	{
		PlayerInputContainer.Instance.playerInputActions.PlayerHull.Enable();
	}

	private void OnDisable()
	{
		PlayerInputContainer.Instance.playerInputActions.PlayerHull.Disable();
	}
	public override Vector2 GetMovementInput()
	{
		movementInput = PlayerInputContainer.Instance.playerInputActions.PlayerHull.Move.ReadValue<Vector2>();
		return movementInput;
	}
}
