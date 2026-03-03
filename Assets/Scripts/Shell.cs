using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shell : MonoBehaviour
{
    [SerializeField] private float damage = 30.0f;
    [SerializeField] private float moveSpeed = 10.0f;
    private BoxCollider2D boxCollider;
    private Vector3 targetCoordinates;
    private float previousDistanceToTarget;
    private float reachedDistanceThreshold = 0.1f;
    private float collisionDistanceThreshold = 5.5f;
	private bool collided = false;
	private bool reachedTarget = false;

	//Following events to be used for the visual component
    public event EventHandler OnReachedTarget;
    public event EventHandler OnHit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        previousDistanceToTarget = Vector3.Distance(transform.position, targetCoordinates);
		boxCollider = GetComponent<BoxCollider2D>();
		boxCollider.enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
		Move();
	}

	private void Move()
	{
		if (!reachedTarget && !collided)
		{
			transform.position += transform.up * moveSpeed * Time.deltaTime;

			float distanceToTarget = Vector3.Distance(transform.position, targetCoordinates);

			if (distanceToTarget < collisionDistanceThreshold && !boxCollider.enabled)
			{
				boxCollider.enabled = true;
			}

			if (distanceToTarget < reachedDistanceThreshold || distanceToTarget > previousDistanceToTarget)
			{
				this.transform.position = targetCoordinates;
				reachedTarget = true;
				OnReachedTarget?.Invoke(this, EventArgs.Empty);
				float destructionTimer = 2.0f;
				Destroy(gameObject, destructionTimer);
			}

			previousDistanceToTarget = distanceToTarget;
		}
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collided)
		{
			if (collision.gameObject.TryGetComponent(out IDamagable damagable))
			{
				damagable.Damage(damage);
			}
			OnHit?.Invoke(this, EventArgs.Empty);
			collided = true;
			if (!reachedTarget)
			{
				OnReachedTarget?.Invoke(this, EventArgs.Empty);
				float destructionTimer = 2.0f;
				Destroy(gameObject, destructionTimer);
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
