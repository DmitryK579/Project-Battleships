using UnityEngine;

public class CPUShipController : ShipController
{
	[Header("Random movement settings")]
	[SerializeField] private float minThrustVector;
	[SerializeField] private float maxMovementVector;

	[Header("Move to target settings")]
	[SerializeField] private float turnDeadzone;

	[Header("Obstacle Detection Settings")]
	[SerializeField] private LayerMask obstacleLayer;
	[SerializeField] private CollisionDetectionRaycast leftDetectionRaycast;
	[SerializeField] private CollisionDetectionRaycast frontDetectionRaycast;
	[SerializeField] private CollisionDetectionRaycast rightDetectionRaycast;

	private Vector2 movementInput = Vector2.zero;

	private struct TurnValues
	{
		public const float TurnRight = 1f;
		public const float TurnLeft = -1f;
	}

	public void SetRandomVector()
	{
		movementInput.y = Random.Range(minThrustVector, maxMovementVector);
		if (movementInput.y > 1f)
			movementInput.y = 1f;

		movementInput.x = Random.Range(-maxMovementVector, maxMovementVector);
		if (movementInput.y > TurnValues.TurnRight)
			movementInput.y = TurnValues.TurnRight;
		else if (movementInput.y < TurnValues.TurnLeft)
			movementInput.y = TurnValues.TurnLeft;
	}

	public void SetVectorTowardTarget(Vector3 target)
	{
		Vector2 directionToTarget = (target - this.transform.position).normalized;

		float crossProduct = (transform.up.x * directionToTarget.y) - (transform.up.y * directionToTarget.x);
		if (crossProduct > turnDeadzone)
		{
			movementInput.x = TurnValues.TurnLeft;
		}
		else if (crossProduct < -turnDeadzone)
		{
			movementInput.x = TurnValues.TurnRight;
		}
		else
		{
			movementInput.x = 0f;
		}
		movementInput.y = 1f;
	}

    public override Vector2 GetMovementInput()
	{
		UpcomingCollisionCheck();
		//Player inputs are currently normalized so this input should be normalized too.
		movementInput.Normalize();
		return movementInput;
    }

	private void UpcomingCollisionCheck()
	{
		bool obstacleFront = CheckRay(frontDetectionRaycast);
		bool obstacleLeft = CheckRay(leftDetectionRaycast);
		bool obstacleRight = CheckRay(rightDetectionRaycast);

		if (obstacleFront || obstacleLeft || obstacleRight)
		{
			if (obstacleLeft && !obstacleRight)
			{ 
				movementInput.x = TurnValues.TurnRight; 
			}
			else if (obstacleRight && !obstacleLeft)
			{ 
				movementInput.x = TurnValues.TurnLeft; 
			}
			else
			{
				movementInput.x = TurnValues.TurnRight;
			}
		}
	}
	private bool CheckRay(CollisionDetectionRaycast raycast)
	{
		RaycastHit2D hit = Physics2D.Raycast(raycast.transform.position, raycast.transform.up, raycast.Range, obstacleLayer);
		Debug.DrawRay(raycast.transform.position, raycast.transform.up * raycast.Range, hit.collider ? Color.red : Color.green);
		return (hit.collider != null);
	}
}
