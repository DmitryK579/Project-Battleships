using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CPUTurretController : TurretController
{
	[SerializeField] private float leadTargetPerUnitDistance;

	private Vector3 targetCoordinates = Vector3.zero;
	private float maxRange = 0f;
	public override Vector3 GetTargetCoordinates()
	{
		return targetCoordinates;
	}

	public void Initialize(float turretMaxRange)
	{
		maxRange = turretMaxRange;
	}
	public void TargettingLogic(Vector3 coordinates, Vector3 direction)
	{
		float distanceToTarget = Vector2.Distance(targetCoordinates, transform.position);
		targetCoordinates = coordinates + (direction * (leadTargetPerUnitDistance * distanceToTarget));

		if (distanceToTarget <= maxRange)
		{
			Debug.Log(distanceToTarget);
			InvokeOnShoot(EventArgs.Empty);
		}
	}
}
