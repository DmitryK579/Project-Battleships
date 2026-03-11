using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TurretStatusManager : MonoBehaviour
{
	[field: SerializeField] public ShipHandler TrackedShip { get; set; }
    [SerializeField] private GameObject turretStatusCirclePrefab;
    [SerializeField] private float initialXOffset;
    [SerializeField] private float initialYOffset;
    [SerializeField] private float xStep;
    [SerializeField] private float yStep;
    [SerializeField] private int circlesPerRow;

    private List<GameObject> turretStatusCircles;
    private Vector3 newCirclePosition;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Start()
    {
        turretStatusCircles = new List<GameObject>();
        Initialize();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void Initialize()
    {
        if (turretStatusCircles.Count == 0)
        {
            newCirclePosition = new Vector3(initialXOffset, initialYOffset, 0);
        }

        List<TurretHandler> turretsToTrack = TrackedShip.GetControllableTurrets();
        for (int i = 0; i < turretsToTrack.Count; i++)
        {
            GameObject turretStatusCircle = Instantiate(turretStatusCirclePrefab, this.transform);
            turretStatusCircle.transform.localPosition = newCirclePosition;
            TurretStatusCircle script = turretStatusCircle.GetComponent<TurretStatusCircle>();
            script.TrackedTurret = turretsToTrack[i];
            
            turretStatusCircles.Add(turretStatusCircle);
            newCirclePosition.x += xStep;
            if ((i+1)%circlesPerRow == 0)
            {
                newCirclePosition.x = initialXOffset;
                newCirclePosition.y += yStep;
            }
        }
    }
}
