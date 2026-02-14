using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTurretController : TurretController
{
	private Vector3 targetCoordinates;
	public override Vector3 GetTargetCoordinates()
	{
		targetCoordinates = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
		targetCoordinates.z = 0;
		return targetCoordinates;
	}
}
