using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShipHandler : MonoBehaviour, IDamagable, IShellBlocker
{
	[Header("Scripts")]
	[SerializeField] private CPUShipController cpuShipController;
	[Header("Ship settings")]
	[field: SerializeField] public ShipScriptableObject Ship { get; private set; }
	[SerializeField] private List<TurretHandler> primaryTurrets;
	[SerializeField] private List<TurretHandler> secondaryTurrets;
	[SerializeField] private float objectHeight = 1.0f;

	[Header("Visual references")]
	[SerializeField] private Image healthBar;
	private float currentHealth;

	private ShipController currentShipController;
	private Vector2 shipMovementVector = Vector2.zero;
	private Rigidbody2D shipRigidbody2D;

	public event EventHandler OnCriticallyDamaged;
	private bool invokedOnCriticallyDamaged;
	public event EventHandler OnZeroHealth;

	private void Awake()
	{
		shipRigidbody2D = GetComponent<Rigidbody2D>();
		currentShipController = cpuShipController;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		currentHealth = Ship.Health;
	}

    // Update is called once per frame
    void Update()
    {
		if (GameManager.Instance.GetGameState() != GameManager.GameState.Playing &&
			GameManager.Instance.GetGameState() != GameManager.GameState.BattleEnd)
			return;

		shipMovementVector = currentShipController.GetMovementInput();
	}

	void FixedUpdate()
	{
		if (GameManager.Instance.GetGameState() != GameManager.GameState.Playing &&
			GameManager.Instance.GetGameState() != GameManager.GameState.BattleEnd)
			return;

		ApplyEngineForce();
		ApplyShipRotation();
		HandleDrift();
	}
	void ApplyEngineForce()
	{
		Vector2 engineForceVector = transform.up * shipMovementVector.y * Ship.AccelerationFactor;
		shipRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);
	}

	void ApplyShipRotation()
	{
		var torque = -shipMovementVector.x * Ship.RotationFactor;
		shipRigidbody2D.AddTorque(torque, ForceMode2D.Force);
	}

	void HandleDrift()
	{
		//Kill orthogonal velocity so ship doesn't drift as if in space
		Vector2 forwardVelocity = transform.up * Vector2.Dot(shipRigidbody2D.linearVelocity, transform.up);
		Vector2 rightVelocity = transform.right * Vector2.Dot(shipRigidbody2D.linearVelocity, transform.right);

		shipRigidbody2D.linearVelocity = forwardVelocity + rightVelocity * Ship.DriftFactor;
	}

	public void Damage(float damage)
	{
		currentHealth -= damage;
		if (currentHealth <= 0)
		{
			currentHealth = 0;
			OnZeroHealth?.Invoke(this, EventArgs.Empty);
			Destroy(transform.parent.gameObject);
		}
		
		if (!invokedOnCriticallyDamaged)
		{
			if (currentHealth < (Ship.Health/4))
			{
				OnCriticallyDamaged?.Invoke(this, EventArgs.Empty);
				invokedOnCriticallyDamaged = true;
			}
		}
	}

	public float GetObjectHeight()
	{
		return objectHeight;
	}

	public float GetCurrentHealth()
	{
		return currentHealth;
	}

	public List<TurretHandler> GetControllableTurrets()
	{
		return primaryTurrets;
	}

	public void ChangeControllerToPlayer(PlayerShipController shipController, PlayerTurretController turretController)
	{
		currentShipController = shipController;
		foreach (TurretHandler turret in primaryTurrets)
		{
			turret.ChangeControllerToPlayer(turretController);
		}
	}

	public void ResetControllerToOwnCPU()
	{
		currentShipController = cpuShipController;
		foreach (TurretHandler turret in primaryTurrets)
		{
			turret.ResetControllerToOwnCPU();
		}
	}

	public void ChangeHealthBarColor(Color color)
	{
		healthBar.color = color;
	}
}
