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
	[SerializeField] private List<Transform> shellSpawners;
	[SerializeField] private GameObject shellPrefab;
	[SerializeField] private bool isPrimaryTurret = true;
	[SerializeField] private float rotationFactor = 1.0f;
    [SerializeField] private float maxRotationAngle = 1.0f;
    [SerializeField] private float maxRange = 600.0f;
    [SerializeField] private float minRange = 30.0f;
    [SerializeField] private float maxDispersion = 10.0f;
    [SerializeField] private float minDispersion = 1.0f;
    [SerializeField] private LineRenderer aimLine;

	private Vector3 targetCoordinates;
	private bool drawLineToTarget = true;

	private const float turretDefaultAngle = 0;
	private float currentRotationAngle = 0;
	private float leftAngleLimit;
	private float rightAngleLimit;
	private float angleFromLimitToCenter;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Start()
    {
		leftAngleLimit = turretDefaultAngle + maxRotationAngle / 2;
		rightAngleLimit = turretDefaultAngle - maxRotationAngle / 2;
		angleFromLimitToCenter = (360 - maxRotationAngle) / 2;

		turretController.OnShoot += Shoot;
	}

    // Update is called once per frame
    private void Update()
    {
		RotateTurret();
		DrawLine();
	}

	private void DrawLine()
	{
		if (drawLineToTarget == true)
		{
			aimLine.enabled = true;
			Vector3 initialPosition = transform.position;
			Vector3 targetPosition = targetCoordinates;
			float lineLength = Mathf.Clamp(Vector2.Distance(initialPosition, targetPosition), minRange, maxRange);
			float turretAimX = initialPosition.x + lineLength * -Mathf.Sin(transform.rotation.eulerAngles.z * (Mathf.PI / 180f));
			float turretAimY = initialPosition.y + lineLength * Mathf.Cos(transform.rotation.eulerAngles.z * (Mathf.PI / 180f));
			Vector3 turretAim = new Vector3(turretAimX, turretAimY, 0);
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

		if (maxRotationAngle != 360)
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
		transform.Rotate(0.0f, 0.0f, rotationFactor * Time.deltaTime);
		if (maxRotationAngle != 360)
		{
			currentRotationAngle += rotationFactor * Time.deltaTime;
			//Handle rotating past the limit due to deltaTime
			if (currentRotationAngle > leftAngleLimit)
			{
				currentRotationAngle = leftAngleLimit;
			}
		}
	}
	private void RotateRight()
	{
		transform.Rotate(0.0f, 0.0f, -rotationFactor * Time.deltaTime);
		if (maxRotationAngle != 360)
		{
			currentRotationAngle -= rotationFactor * Time.deltaTime;
			//Handle rotating past the limit due to deltaTime
			if (currentRotationAngle < rightAngleLimit)
			{
				currentRotationAngle = rightAngleLimit;
			}
		}
	}

	private void Shoot(object sender, EventArgs e)
	{
		float distanceToTarget = Vector3.Distance(transform.position, targetCoordinates);
		float dispersionFactor = distanceToTarget / maxRange;
		float dispersion = Mathf.Clamp((maxDispersion * dispersionFactor), minDispersion, maxDispersion);

		foreach (var spawner in shellSpawners)
		{
			Vector3 modifiedTargetCoordinates = targetCoordinates;
			modifiedTargetCoordinates.x = modifiedTargetCoordinates.x + Random.Range(-dispersion, dispersion);
			modifiedTargetCoordinates.y = modifiedTargetCoordinates.y + Random.Range(-dispersion, dispersion);

			GameObject shell = Instantiate(shellPrefab, spawner.transform.position, this.transform.rotation);
			Shell shellScript = shell.GetComponent<Shell>();
			shellScript.SetTargetCoordinates(modifiedTargetCoordinates);
		}
	}
}
