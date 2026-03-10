using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TurretHandler : MonoBehaviour
{
	[Header("Scripts")]
	[SerializeField] private TurretController turretController;
	[Header("Turret settings")]
	[SerializeField] private TurretScriptableObject turret;
	[SerializeField] private List<Transform> shellSpawners;
    [SerializeField] private LineRenderer aimLine;

	private Vector3 targetCoordinates;
	private Vector3 turretAim = Vector3.zero;
	private bool drawLineToTarget = true;

	private const float turretDefaultAngle = 0;
	private float currentRotationAngle = 0;
	private float leftAngleLimit;
	private float rightAngleLimit;
	private float angleFromLimitToCenter;
	private float reloadTimerS;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Start()
    {
		leftAngleLimit = turretDefaultAngle + turret.MaxRotationAngle / 2;
		rightAngleLimit = turretDefaultAngle - turret.MaxRotationAngle / 2;
		angleFromLimitToCenter = (360 - turret.MaxRotationAngle) / 2;
		reloadTimerS = 0.0f;

		turretController.OnShoot += Shoot;
	}

    // Update is called once per frame
    private void Update()
    {
		RotateTurret();
		DrawLine();
		if (reloadTimerS>0)
			reloadTimerS -= Time.deltaTime;
	}

	private void DrawLine()
	{
		Vector3 initialPosition = transform.position;
		Vector3 targetPosition = targetCoordinates;
		float lineLength = Mathf.Clamp(Vector2.Distance(initialPosition, targetPosition), turret.MinRange, turret.MaxRange);
		turretAim.x = initialPosition.x + lineLength * -Mathf.Sin(transform.rotation.eulerAngles.z * (Mathf.PI / 180f));
		turretAim.y = initialPosition.y + lineLength * Mathf.Cos(transform.rotation.eulerAngles.z * (Mathf.PI / 180f));

		if (drawLineToTarget == true)
		{
			aimLine.enabled = true;
			aimLine.SetPosition(0, initialPosition);
			aimLine.SetPosition(1, turretAim);
			//Debug.Log("IP: " + initialPosition);
			//Debug.Log("TP: " + targetPosition);
		}
		else
		{
			aimLine.enabled = false;
		}
	}

	private void RotateTurret()
	{
		targetCoordinates = turretController.GetTargetCoordinates();

		Vector3 frontVectorToTarget = (targetCoordinates - transform.position).normalized;
		float angleToTarget = -(Vector3.SignedAngle(frontVectorToTarget, transform.up, new Vector3(0, 0, 1)));

		if (turret.MaxRotationAngle != 360)
		{
			if (angleToTarget > 0)
			{
				float angleFromTurretToLimit = leftAngleLimit - currentRotationAngle;

				if (angleToTarget > angleFromTurretToLimit + angleFromLimitToCenter)
				{
					RotateRight();
				}

				else if (currentRotationAngle < leftAngleLimit)
				{
					RotateLeft();
				}
			}
			else if (angleToTarget < 0)
			{
				float angleFromTurretToLimit = rightAngleLimit - currentRotationAngle;

				if (angleToTarget < angleFromTurretToLimit - angleFromLimitToCenter)
				{
					RotateLeft();
				}
				else if (currentRotationAngle > rightAngleLimit)
				{
					RotateRight();
				}
			}
		}
		else
		{
			if (angleToTarget > 0)
			{
				RotateLeft();
			}
			else if (angleToTarget < 0)
			{
				RotateRight();
			}
		}
	}
	private void RotateLeft()
	{
		transform.Rotate(0.0f, 0.0f, turret.RotationSpeed * Time.deltaTime);
		if (turret.MaxRotationAngle != 360)
		{
			currentRotationAngle += turret.RotationSpeed * Time.deltaTime;
			//Handle rotating past the limit due to deltaTime
			if (currentRotationAngle > leftAngleLimit)
			{
				currentRotationAngle = leftAngleLimit;
			}
		}
	}
	private void RotateRight()
	{
		transform.Rotate(0.0f, 0.0f, -turret.RotationSpeed * Time.deltaTime);
		if (turret.MaxRotationAngle != 360)
		{
			currentRotationAngle -= turret.RotationSpeed * Time.deltaTime;
			//Handle rotating past the limit due to deltaTime
			if (currentRotationAngle < rightAngleLimit)
			{
				currentRotationAngle = rightAngleLimit;
			}
		}
	}

	private void Shoot(object sender, EventArgs e)
	{
		if (reloadTimerS > 0)
		{
			return;
		}

		float distanceToTarget = Vector3.Distance(transform.position, turretAim);
		float dispersionFactor = distanceToTarget / turret.MaxRange;
		float dispersion = Mathf.Clamp((turret.MaxDispersion * dispersionFactor), turret.MinDispersion, turret.MaxDispersion);

		foreach (var spawner in shellSpawners)
		{
			Vector3 modifiedTargetCoordinates = turretAim;
			modifiedTargetCoordinates.x = modifiedTargetCoordinates.x + Random.Range(-dispersion, dispersion);
			modifiedTargetCoordinates.y = modifiedTargetCoordinates.y + Random.Range(-dispersion, dispersion);

			GameObject shell = Instantiate(turret.ShellScriptableObject.Prefab, spawner.transform.position, this.transform.rotation);
			Shell shellScript = shell.GetComponent<Shell>();
			shellScript.Initialize(modifiedTargetCoordinates,turret.MaxRange);
		}

		reloadTimerS = turret.ReloadTimeS;
	}

	public (float reloadTime, float reloadTimer) GetReloadTimeAndTimer()
	{
		return (turret.ReloadTimeS,reloadTimerS);
	}
}
