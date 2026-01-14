using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float acceleration = 1.0f;
    [SerializeField] private float rotationAcceleration = 1.0f;
    private Rigidbody2D rb;
    PlayerInputActions playerInputActions;


	private void Awake()
	{
        playerInputActions = new PlayerInputActions();
		playerInputActions.Player.Enable();
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
		Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
		ApplyEngineForce(inputVector);
        ApplyShipRotation(inputVector);
	}

    void ApplyEngineForce(Vector2 inputVector)
    {
		Vector2 engineForceVector = transform.up * inputVector.y * acceleration;
		rb.AddForce(engineForceVector, ForceMode2D.Force);
	}

    void ApplyShipRotation(Vector2 inputVector)
    {
		var torque = -inputVector.x * rotationAcceleration;
		rb.AddTorque(torque, ForceMode2D.Force);
	}
}
