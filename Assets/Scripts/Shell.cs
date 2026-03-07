using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shell : MonoBehaviour
{
	[SerializeField] ShellScriptableObject shell;
    private BoxCollider2D boxCollider;
	private Vector3 initialCoordinates;
    private Vector3 targetCoordinates;
	private float travelProgress = 0.0f;
    private float previousDistanceToTarget;
    private float distanceToTravel;
	private float currentHeight;
	private float turretMaxRange;
	private float armingTimer = 0.0f;
	private bool collided = false;
	private bool reachedTarget = false;

	//Following events to be used for the visual component
    public event EventHandler OnReachedTarget;
    public event EventHandler OnHit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
		initialCoordinates = transform.position;
		distanceToTravel = Vector3.Distance(initialCoordinates, targetCoordinates);
		previousDistanceToTarget = distanceToTravel;
		boxCollider = GetComponent<BoxCollider2D>();
		boxCollider.enabled = false;
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
			travelProgress += (shell.Speed * Time.deltaTime) / Vector3.Distance(initialCoordinates, targetCoordinates);
			transform.position = Vector3.Lerp(initialCoordinates, targetCoordinates, travelProgress);
			currentHeight = Mathf.Sin(travelProgress * Mathf.PI) * (shell.MaxHeight * (distanceToTravel/turretMaxRange));

			if (travelProgress >= 1)
			{
				this.transform.position = targetCoordinates;
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
				if (currentHeight > blocker.GetObjectHeight())
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
        targetCoordinates = coordinates;
        RotateToTargetCoordinates();
		turretMaxRange = maxRange;
	}

    private void RotateToTargetCoordinates()
    {
		Vector3 direction = targetCoordinates - transform.position;
		transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
	}
}
