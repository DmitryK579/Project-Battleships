using UnityEngine;

public class TurretHandler : MonoBehaviour
{
	[Header("Scripts")]
	[SerializeField] private TurretController turretController;
	[Header("Turret settings")]
	[SerializeField] private bool isPrimaryTurret = true;
	[SerializeField] private float rotationFactor = 1.0f;
    [SerializeField] private float maxRotationAngle = 1.0f;
    [SerializeField] private LineRenderer aimLine;

	private Vector3 targetCoordinates;
	private bool drawLineToTarget = true;

	private const float turretDefaultAngle = 0;
	private float currentRotationAngle = 0;
	private float leftAngleLimit;
	private float rightAngleLimit;
	private float angleFromLimitToCenter;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		leftAngleLimit = turretDefaultAngle + maxRotationAngle / 2;
		rightAngleLimit = turretDefaultAngle - maxRotationAngle / 2;
		angleFromLimitToCenter = (360 - maxRotationAngle) / 2;
	}

    // Update is called once per frame
    void Update()
    {
		RotateTurret();
		DrawLine();
	}

	void DrawLine()
	{
		if (drawLineToTarget == true)
		{
			aimLine.enabled = true;
			Vector3 initialPosition = transform.position;
			Vector3 targetPosition = targetCoordinates;
			//Vector3 frontVectorToTarget = (targetCoordinates - transform.position).normalized;
			float lineMax = 200;
			float lineLength = Mathf.Clamp(Vector2.Distance(initialPosition, targetPosition), 0, lineMax);
			//targetPosition = initialPosition + (frontVectorToTarget * lineLength);
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

	void RotateTurret()
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
	void RotateLeft()
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
	void RotateRight()
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

}
