using System.Collections.Generic;
using UnityEngine;

public class CPULogic : MonoBehaviour
{
	[Header("Scripts")]
    [SerializeField] private CPUShipController shipController;
    [SerializeField] private CPUTurretController turretController;
    [SerializeField] private ShipHandler ownShip;

	[Header("Decision settings")]
	[SerializeField] private float maxDecisionCooldownS;
	[SerializeField] private float minDecisionCooldownS;

    [Header("Random movement settings")]
	[SerializeField] private float vectorChangeCooldownS;

	[Header("Target settings")]
	[SerializeField] private float beginTurretTargettingAtDistance;
	[field: SerializeField, Header("Mutable")] public ShipHandler TargetShip { get; private set; }

	private enum MovementStates
    {
        Idle,
        Random,
        MoveToTarget,
    }

    private MovementStates movementState = MovementStates.Idle;

    private enum TurretStates
    {
        Idle,
        TargetEnemy,
    }

    private TurretStates turretState = TurretStates.Idle;

	private float vectorChangeTimer = 0f;
	private float decisionTimer = 0f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		MakeDecisions();
	}

    // Update is called once per frame
    void Update()
    {
		if (TargetShip == null)
			MakeDecisions();

		decisionTimer -= Time.deltaTime;
		if (decisionTimer <= 0f)
		{
			MakeDecisions();
		}

		MovementLogic();
		TurretLogic();
    }
	private void MakeDecisions()
	{
		decisionTimer = Random.Range(maxDecisionCooldownS, maxDecisionCooldownS);

		List<ShipHandler> validTargets = GameManager.Instance.GetTargetShips(ownShip);
		float closestDitanceToTarget = 0;
		foreach (ShipHandler target in validTargets)
		{
			if (target == null) 
				continue;

			float distance = Vector2.Distance(target.gameObject.transform.position, transform.position);
			if (closestDitanceToTarget == 0 || distance < closestDitanceToTarget)
			{
				closestDitanceToTarget = distance;
				TargetShip = target;
			}
		}

		UpdateMovementState();
		UpdateTurretState();
	}

	private void MovementLogic()
    {
		switch (movementState)
		{
			case MovementStates.Idle:
				{
					break;
				}
			case MovementStates.Random:
				{
					if (vectorChangeTimer > 0)
					{
						vectorChangeTimer -= Time.deltaTime;
						break;
					}
					shipController.SetRandomVector();
					vectorChangeTimer = vectorChangeCooldownS;
					break;
				}
			case MovementStates.MoveToTarget:
				{
					shipController.SetVectorTowardTarget(TargetShip.transform.position);
					break;
				}
		}
	}

    private void TurretLogic()
    {
		UpdateTurretState();
		switch (turretState)
		{
			case TurretStates.Idle:
				{
					break;
				}
			case TurretStates.TargetEnemy:
				{
					turretController.TargettingLogic(TargetShip.transform.position, TargetShip.transform.up);
					break;
				}
		}
	}

	private void UpdateMovementState()
	{
		if (TargetShip == null)
		{
			movementState = MovementStates.Idle;
			return;
		}

		if (Vector3.Distance(transform.position, TargetShip.transform.position) > (beginTurretTargettingAtDistance / 2))
			movementState = MovementStates.MoveToTarget;
		else
			movementState = MovementStates.Random;
	}

	private void UpdateTurretState()
	{
		if (TargetShip == null)
		{
			SetTurretsToIdle();
			return;
		}

		if (Vector3.Distance(transform.position, TargetShip.transform.position) > beginTurretTargettingAtDistance)
			SetTurretsToIdle();
		else
			SetTurretsToNoLongerIdle();
	}

	private void SetTurretsToIdle()
	{
		turretState = TurretStates.Idle;
		turretController.SetTurretToIdle();
	}

	private void SetTurretsToNoLongerIdle()
	{
		turretState = TurretStates.TargetEnemy;
		turretController.SetTurretToNoLongerIdle();
	}
}
