using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shell : MonoBehaviour
{
	[SerializeField] ShellScriptableObject shell;
    private BoxCollider2D boxCollider;
	private ShellMovementHandler movementHandler;
	private float armingTimer = 0.0f;
	private bool collided = false;
	private bool reachedTarget = false;

	//Following events to be used for the visual component
    public event EventHandler OnReachedTarget;
    public event EventHandler OnHit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
		ArmShell();
		Move();

	}

	private void Move()
	{
		if (!reachedTarget && !collided)
		{
			movementHandler.Step(Time.deltaTime);
			this.transform.position = movementHandler.CurrentPosition;

			if (movementHandler.TravelProgress >= 1)
			{
				this.transform.position = movementHandler.TargetPosition;
				EndOfFlight();
			}
		}
	}
	//Wait until set time passed before activating collisions to prevent shells from colliding with parent ship
	private void ArmShell()
	{
		if (armingTimer < shell.ArmingTimeS)
		{
			armingTimer += Time.deltaTime;
			if (armingTimer >= shell.ArmingTimeS)
			{
				boxCollider.enabled = true;
			}
		}
	}

	private void EndOfFlight()
	{
		reachedTarget = true;
		OnReachedTarget?.Invoke(this, EventArgs.Empty);
		Destroy(gameObject, shell.FloatTimeS);
	}
	private void OnTriggerStay2D(Collider2D collision)
	{
		if (!collided)
		{
			if (collision.gameObject.TryGetComponent(out IShellBlocker blocker))
			{
				if (movementHandler.CurrentHeight > blocker.GetObjectHeight())
				{
					return;
				}

			}

			Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, shell.ExplosionRadius);
			foreach (Collider2D collider in objectsInRange)
			{
				if (collider.gameObject.TryGetComponent(out IDamagable damagable))
				{
					damagable.Damage(shell.Damage);
				}
			}

			OnHit?.Invoke(this, EventArgs.Empty);
			collided = true;
			if (!reachedTarget)
			{
				EndOfFlight();
			}
		}
	}

	public void Initialize(Vector3 coordinates, float maxRange)
    {
        RotateToTargetCoordinates(coordinates);
		movementHandler = new ShellMovementHandler(transform.position, coordinates, shell.Speed, maxRange, shell.MaxHeight);
		boxCollider = GetComponent<BoxCollider2D>();
		boxCollider.enabled = false;
		this.gameObject.SetActive(true);
	}

    private void RotateToTargetCoordinates(Vector3 coordinates)
    {
		Vector3 direction = coordinates - transform.position;
		transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
	}
}

public class ShellMovementHandler
{
	public Vector2 StartPosition { get; private set; }
	public Vector2 TargetPosition { get; private set; }
	public float ShellSpeed { get; private set; }

	public float MaxRange { get; private set; }
	public float MaxHeight { get; private set; }

	public Vector2 CurrentPosition { get; private set; }
	public float DistanceFromStartToTarget {  get; private set; }

	public float TravelProgress { get; private set; }

	public float CurrentHeight { get; private set; }

	public ShellMovementHandler(Vector2 startPosition, Vector2 targetPosition, float shellSpeed, float maxRange, float maxHeight)
	{
		StartPosition = startPosition;
		TargetPosition = targetPosition;
		ShellSpeed = shellSpeed;
		MaxRange = maxRange;
		MaxHeight = maxHeight;
		CurrentPosition = Vector2.zero;
		DistanceFromStartToTarget = Vector2.Distance(StartPosition, TargetPosition);
		TravelProgress = 0f;
		CurrentHeight = 0f;
	}

	public void Step(float frameInterval = 1f)
	{
		TravelProgress += (ShellSpeed * frameInterval) / Vector2.Distance(StartPosition, TargetPosition);
		CurrentPosition = Vector2.Lerp(StartPosition, TargetPosition, TravelProgress);
		CurrentHeight = Mathf.Sin(TravelProgress * Mathf.PI) * (MaxHeight * (DistanceFromStartToTarget / MaxRange));
	}

}