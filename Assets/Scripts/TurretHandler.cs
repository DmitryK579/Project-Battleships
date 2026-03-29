using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class TurretHandler : MonoBehaviour
{
	[Header("Scripts")]
	[SerializeField] private CPUTurretController cpuTurretController;
	[Header("Turret settings")]
	[SerializeField] private TurretScriptableObject turret;
	[SerializeField] private List<Transform> shellSpawners;
    [SerializeField] private LineRenderer aimLine;
    [SerializeField] private Transform idleTarget;

	private TurretController currentTurretController;
	private Vector3 targetCoordinates;
	private Vector3 turretAim = Vector3.zero;
	private bool drawLineToTarget = false;

	private float halfOfRotationArc;
	private float reloadTimerS;
	private float initialLocalAngle;
	private float facingThresholdDegrees = 1.0f;
	private bool facingTarget = false;
	private bool idle = true;
	private bool isSimulationEnabled = false;

	public event EventHandler OnShoot;
	public event EventHandler OnFacingTarget;
	public event EventHandler OnNoLongerFacingTarget;

	public event EventHandler<OnShellSimulationCollisionArgs> OnShellSimulationCollision;
	public class OnShellSimulationCollisionArgs : EventArgs
	{
		public Vector3 collisionPosition;
	}
	public event EventHandler OnShellSimulationPass;

	private void Awake()
	{
		currentTurretController = cpuTurretController;
		cpuTurretController.Initialize(turret.MaxRange);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Start()
    {
		halfOfRotationArc = turret.MaxRotationAngle/2f;
		reloadTimerS = 0.0f;

		initialLocalAngle = transform.localEulerAngles.z;
		if (initialLocalAngle > 180) 
			initialLocalAngle -= 360f;

		GameManager.Instance.OnCountdownEnd += NoLongerIdle;
		SubscribeToEvents();
	}

	private void OnDisable()
	{
		GameManager.Instance.OnCountdownEnd -= NoLongerIdle;
		UnsubscribeFromEvents();
	}

	private void SubscribeToEvents()
	{
		currentTurretController.OnShoot += Shoot;
		currentTurretController.OnIdle += Idle;
		currentTurretController.OnNoLongerIdle += NoLongerIdle;
	}

	private void UnsubscribeFromEvents()
	{
		currentTurretController.OnShoot -= Shoot;
		currentTurretController.OnIdle -= Idle;
		currentTurretController.OnNoLongerIdle -= NoLongerIdle;
	}

	// Update is called once per frame
	private void Update()
    {
		if (GameManager.Instance.GetGameState() != GameManager.GameState.Playing &&
			GameManager.Instance.GetGameState() != GameManager.GameState.BattleEnd)
			return;

		RotateTurret();
		DrawLine();
		if (isSimulationEnabled)
			SimulateShell();
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
		}
		else
		{
			aimLine.enabled = false;
		}
	}

	private void RotateTurret()
	{
		if (!idle)
			targetCoordinates = currentTurretController.GetTargetCoordinates();
		else
			targetCoordinates = idleTarget.transform.position;

		Vector3 direction = targetCoordinates - transform.position;

		//Get the world target angle and subtract parent rotation to get local target.
		float globalTargetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90.0f;
		float parentRotation = transform.parent.eulerAngles.z;
		float localTargetAngle = Mathf.DeltaAngle(parentRotation, globalTargetAngle);

		//Calculate how far the target is from our starting orientation.
		//DeltaAngle handles the 180/-180 wrap-around.
		float targetOffsetFromStart = Mathf.DeltaAngle(initialLocalAngle, localTargetAngle);

		//Clamp the offset within the allowed arc.
		float clampedOffset = Mathf.Clamp(targetOffsetFromStart, -halfOfRotationArc, halfOfRotationArc);

		//Calculate our current offset from the start.
		float currentLocalAngle = transform.localEulerAngles.z;
		if (currentLocalAngle > 180) 
			currentLocalAngle -= 360f;
		float currentOffsetFromStart = Mathf.DeltaAngle(initialLocalAngle, currentLocalAngle);

		//Move toward the clamped offset using linear math (no wrapping).
		//This prevents the "long way around" snapping.
		float nextOffset = Mathf.MoveTowards(
			currentOffsetFromStart,
			clampedOffset,
			turret.RotationSpeed * Time.deltaTime
		);

		//Re-apply the offset to the starting angle.
		float finalAngle = initialLocalAngle + nextOffset;
		transform.localRotation = Quaternion.Euler(0, 0, finalAngle);

		//Check whether the turret is facing the target.
		if (Mathf.Abs(Mathf.DeltaAngle(finalAngle, localTargetAngle)) < facingThresholdDegrees &&
			Vector2.Distance(transform.position, targetCoordinates) <= turret.MaxRange)
		{
			facingTarget = true;
			OnFacingTarget?.Invoke(this, EventArgs.Empty);
		}
		else if (facingTarget)
		{
			facingTarget = false;
			OnNoLongerFacingTarget?.Invoke(this, EventArgs.Empty);
		}
	}

	private void Shoot(object sender, EventArgs e)
	{
		if (reloadTimerS > 0 || !facingTarget)
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
		OnShoot?.Invoke(this, EventArgs.Empty);
	}

	private void SimulateShell()
	{
		float simulationStep = 1f;
		float shellHitBoxSize = 0.1f;
		float distance = Mathf.Clamp(Vector2.Distance(transform.position, turretAim), turret.MinRange, turret.MaxRange);
		bool collided = false;

		if (distance < 50)
		{
			simulationStep = simulationStep/10f;
		}

		ShellMovementHandler shellMovementSimulation = new ShellMovementHandler(transform.position, turretAim, simulationStep, turret.MaxRange, turret.ShellScriptableObject.MaxHeight);

		for (float i = 0f; i < distance; i+=simulationStep)
		{
			shellMovementSimulation.Step();

			if (Vector2.Distance(shellMovementSimulation.CurrentPosition, transform.position) > turret.MinRange)
			{
				Collider2D[] objectsInRange = Physics2D.OverlapBoxAll(shellMovementSimulation.CurrentPosition, new Vector2(shellHitBoxSize, shellHitBoxSize), 0f);
				foreach (Collider2D collider in objectsInRange)
				{
					if (collider.gameObject.TryGetComponent(out IShellBlocker blocker))
					{
						if (blocker.GetObjectHeight() >= shellMovementSimulation.CurrentHeight)
						{
							collided = true;
						}
					}
				}
			}

			if (collided)
			{
				OnShellSimulationCollision?.Invoke(this, new OnShellSimulationCollisionArgs { collisionPosition = shellMovementSimulation.CurrentPosition });
				break;
			}
		}

		if (!collided)
			OnShellSimulationPass?.Invoke(this, EventArgs.Empty);
	}

	public (float reloadTime, float reloadTimer) GetReloadTimeAndTimer()
	{
		return (turret.ReloadTimeS,reloadTimerS);
	}

	private void Idle(object sender, EventArgs e)
	{
		idle = true;
	}
	private void NoLongerIdle(object sender, EventArgs e)
	{
		idle = false;
	}

	public void ChangeControllerToPlayer(PlayerTurretController controller)
	{
		UnsubscribeFromEvents();
		currentTurretController = controller;
		SubscribeToEvents();
		isSimulationEnabled = true;
		if (GameManager.Instance.GetGameState() == GameManager.GameState.Playing)
			idle = false;
	}

	public void ResetControllerToOwnCPU()
	{
		UnsubscribeFromEvents();
		currentTurretController = cpuTurretController;
		SubscribeToEvents();
		isSimulationEnabled = false;
	}
}
