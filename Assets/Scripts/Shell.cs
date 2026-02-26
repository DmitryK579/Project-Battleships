using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10.0f;
    private Vector3 targetCoordinates;
    private float previousDistanceToTarget;
    private float distanceThreshold = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        previousDistanceToTarget = Vector3.Distance(transform.position, targetCoordinates);
    }

    // Update is called once per frame
    private void Update()
    {
		transform.position += transform.up * moveSpeed * Time.deltaTime;

        float distanceToTarget = Vector3.Distance(transform.position, targetCoordinates);
        if (distanceToTarget < distanceThreshold || distanceToTarget > previousDistanceToTarget)
        {
            transform.position = targetCoordinates;
            Destroy(gameObject);
        }

        previousDistanceToTarget = distanceToTarget;
	}

    public void SetTargetCoordinates(Vector3 coordinates)
    {
        targetCoordinates = coordinates;
    }
}
