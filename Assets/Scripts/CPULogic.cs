using UnityEngine;

public class CPULogic : MonoBehaviour
{
	[Header("Scripts")]
	[SerializeField] private GameManager gameManager;
    [SerializeField] private CPUShipController shipController;
    [SerializeField] private CPUTurretController turretController;

    [Header("Random movement settings")]
	[SerializeField] private float vectorChangeCooldownS;

	[field: SerializeField, Header("Mutable")] public ShipHandler TargetShip { get; private set; }

	private enum MovementStates
    {
        Idle,
        Random,
        MoveToTarget,
    }

    private MovementStates movementState = MovementStates.MoveToTarget;

    private enum TurretStates
    {
        Idle,
        TargetEnemy,
    }

    private TurretStates turretState = TurretStates.Idle;

	private float vectorChangeTimer = 0f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		MovementLogic();
		TurretLogic();
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
		switch (turretState)
		{
			case TurretStates.Idle:
				{
					//TODO: turretController call
					break;
				}
			case TurretStates.TargetEnemy:
				{
					//TODO: turretController call (send target coordinates)
					break;
				}
		}
	}
}
