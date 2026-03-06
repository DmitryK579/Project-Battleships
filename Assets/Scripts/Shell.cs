using System;
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
			float height = Mathf.Sin(travelProgress * Mathf.PI) * (shell.MaxHeight * (distanceToTravel/600.0f));

			if (travelProgress >= 1)
			{
				this.transform.position = targetCoordinates;
				reachedTarget = true;
				OnReachedTarget?.Invoke(this, EventArgs.Empty);
				float destructionTimer = 2.0f;
				Destroy(gameObject, destructionTimer);
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
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collided)
		{
			if (collision.gameObject.TryGetComponent(out IDamagable damagable))
			{
				damagable.Damage(shell.Damage);
			}
			OnHit?.Invoke(this, EventArgs.Empty);
			collided = true;
			if (!reachedTarget)
			{
				OnReachedTarget?.Invoke(this, EventArgs.Empty);
				Destroy(gameObject, shell.FloatTimeS);
			}
		}
	}

	public void SetTargetCoordinates(Vector3 coordinates)
    {
        targetCoordinates = coordinates;
        RotateToTargetCoordinates();
	}

    private void RotateToTargetCoordinates()
    {
		Vector3 direction = targetCoordinates - transform.position;
		transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
	}
}
