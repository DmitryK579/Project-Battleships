using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretStatusManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject turretStatusCirclePrefab;
    [SerializeField] private GameObject turretSimulationTrackerPrefab;
    [SerializeField] private float initialXOffset;
    [SerializeField] private float initialYOffset;
    [SerializeField] private float xStep;
    [SerializeField] private float yStep;
    [SerializeField] private int circlesPerRow;

    private List<GameObject> turretStatusCircles;
    private List<GameObject> turretSimulationTrackers;
    private Vector3 newCirclePosition;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	private void Start()
    {
		Initialize();
        gameManager.OnPlayerShipSwap += OnPlayerShipSwap;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

	private void OnDisable()
	{
		gameManager.OnPlayerShipSwap -= OnPlayerShipSwap;
	}

	private void Initialize()
    {

		turretStatusCircles = new List<GameObject>();
		turretSimulationTrackers = new List<GameObject>();
		newCirclePosition = new Vector3(initialXOffset, initialYOffset, 0);

        List<TurretHandler> turretsToTrack = gameManager.PlayerShip.GetControllableTurrets();
        for (int i = 0; i < turretsToTrack.Count; i++)
        {
            GameObject turretStatusCircle = Instantiate(turretStatusCirclePrefab, this.transform);
            turretStatusCircle.transform.localPosition = newCirclePosition;
            TurretStatusCircle statusScript = turretStatusCircle.GetComponent<TurretStatusCircle>();
            statusScript.Initialize(turretsToTrack[i]);
            
            turretStatusCircles.Add(turretStatusCircle);
            newCirclePosition.x += xStep;
            if ((i+1)%circlesPerRow == 0)
            {
                newCirclePosition.x = initialXOffset;
                newCirclePosition.y += yStep;
            }

            GameObject turretSimulationTracker = Instantiate(turretSimulationTrackerPrefab, this.transform);
            TurretSimulationTracker trackerScript = turretSimulationTracker.GetComponent<TurretSimulationTracker>();
			trackerScript.Initialize(turretsToTrack[i]);
			turretSimulationTrackers.Add(turretSimulationTracker);
        }
    }

    private void OnPlayerShipSwap(object sender, EventArgs e)
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }

        Initialize();
    }
}
