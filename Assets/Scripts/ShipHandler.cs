using UnityEngine;

public class ShipHandler : MonoBehaviour, IDamagable, IShellBlocker
{
	[Header("Scripts")]
	[SerializeField] private ShipController shipController;
	[Header("Ship settings")]
	[SerializeField] private float health = 60000.0f;
	[SerializeField] private float accelerationFactor = 1.0f;
	[SerializeField] private float rotationFactor = 1.0f;
	[SerializeField] private float driftFactor = 1.0f;
	[SerializeField] private float objectHeight = 1.0f;

	private Rigidbody2D shipRigidbody2D;

	private void Awake()
	{
		shipRigidbody2D = GetComponent<Rigidbody2D>();
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
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
		Vector2 engineForceVector = transform.up * shipController.GetMovementInput().y * accelerationFactor;
		shipRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);
	}

	void ApplyShipRotation()
	{
		var torque = -shipController.GetMovementInput().x * rotationFactor;
		shipRigidbody2D.AddTorque(torque, ForceMode2D.Force);
	}

	void HandleDrift()
	{
		//Kill orthogonal velocity so ship doesn't drift as if in space
		Vector2 forwardVelocity = transform.up * Vector2.Dot(shipRigidbody2D.linearVelocity, transform.up);
		Vector2 rightVelocity = transform.right * Vector2.Dot(shipRigidbody2D.linearVelocity, transform.right);

		shipRigidbody2D.linearVelocity = forwardVelocity + rightVelocity * driftFactor;
	}

	public void Damage(float damage)
	{
		health -= damage;
	}

	public float GetObjectHeight()
	{
		return objectHeight;
	}
}
