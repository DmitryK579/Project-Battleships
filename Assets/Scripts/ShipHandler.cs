using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShipHandler : MonoBehaviour, IDamagable, IShellBlocker
{
	[Header("Scripts")]
	[SerializeField] private ShipController shipController;
	[Header("Ship settings")]
	[SerializeField] private ShipScriptableObject ship;
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
		currentHealth = ship.Health;
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
		Vector2 engineForceVector = transform.up * shipController.GetMovementInput().y * ship.AccelerationFactor;
		shipRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);
	}

	void ApplyShipRotation()
	{
		var torque = -shipController.GetMovementInput().x * ship.RotationFactor;
		shipRigidbody2D.AddTorque(torque, ForceMode2D.Force);
	}

	void HandleDrift()
	{
		//Kill orthogonal velocity so ship doesn't drift as if in space
		Vector2 forwardVelocity = transform.up * Vector2.Dot(shipRigidbody2D.linearVelocity, transform.up);
		Vector2 rightVelocity = transform.right * Vector2.Dot(shipRigidbody2D.linearVelocity, transform.right);

		shipRigidbody2D.linearVelocity = forwardVelocity + rightVelocity * ship.DriftFactor;
	}

	public void Damage(float damage)
	{
		currentHealth -= damage;
	}

	public float GetObjectHeight()
	{
		return objectHeight;
	}

	public List<TurretHandler> GetControllableTurrets()
	{
		return primaryTurrets;
	}
}
