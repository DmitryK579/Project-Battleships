using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShipHandler : MonoBehaviour, IDamagable, IShellBlocker
{
	[Header("Scripts")]
	[SerializeField] private ShipController shipController;
	[Header("Ship settings")]
	[field: SerializeField] public ShipScriptableObject Ship { get; private set; }
	[SerializeField] private List<TurretHandler> primaryTurrets;
	[SerializeField] private List<TurretHandler> secondaryTurrets;
	[SerializeField] private float objectHeight = 1.0f;
	private float currentHealth;

	private Rigidbody2D shipRigidbody2D;

	private void Awake()
	{
		shipRigidbody2D = GetComponent<Rigidbody2D>();
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		currentHealth = Ship.Health;
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	void FixedUpdate()
	{
		ApplyEngineForce();
		ApplyShipRotation();
		HandleDrift();
	}
	void ApplyEngineForce()
	{
		Vector2 engineForceVector = transform.up * shipController.GetMovementInput().y * Ship.AccelerationFactor;
		shipRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);
	}

	void ApplyShipRotation()
	{
		var torque = -shipController.GetMovementInput().x * Ship.RotationFactor;
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
		if (currentHealth < 0)
			currentHealth = 0;
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
}
