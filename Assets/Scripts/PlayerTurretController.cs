using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTurretController : TurretController
{
	private Vector3 targetCoordinates;

	private void Start()
	{
		PlayerInputContainer.Instance.playerInputActions.PlayerTurret.Enable();
		PlayerInputContainer.Instance.playerInputActions.PlayerTurret.Shoot.performed += OnShootPerformed;
	}

	private void OnDisable()
	{
		PlayerInputContainer.Instance.playerInputActions.PlayerTurret.Disable();
		PlayerInputContainer.Instance.playerInputActions.PlayerTurret.Shoot.performed -= OnShootPerformed;
	}

	private void OnShootPerformed(InputAction.CallbackContext context)
	{
		InvokeOnShoot(EventArgs.Empty);
	}

	public override Vector3 GetTargetCoordinates()
	{
		targetCoordinates = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
		targetCoordinates.z = 0;
		return targetCoordinates;
	}
}
