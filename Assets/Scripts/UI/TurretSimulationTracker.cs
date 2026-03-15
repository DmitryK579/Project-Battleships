using System;
using UnityEngine;
using UnityEngine.UI;

public class TurretSimulationTracker : MonoBehaviour
{
	[field: SerializeField] public TurretHandler TrackedTurret { get; set; }
	[SerializeField] private Image collisionMarker;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		TrackedTurret.OnShellSimulationCollision += OnTurretShellSimulationCollision;
		TrackedTurret.OnShellSimulationPass += OnTurretShellSimulationPass;
	}

	private void OnDisable()
	{
		TrackedTurret.OnShellSimulationCollision -= OnTurretShellSimulationCollision;
		TrackedTurret.OnShellSimulationPass -= OnTurretShellSimulationPass;
	}
	// Update is called once per frame
	void Update()
    {
        
    }

	private void OnTurretShellSimulationPass(object sender, EventArgs e)
	{
		collisionMarker.enabled = false;
	}

	private void OnTurretShellSimulationCollision(object sender, TurretHandler.OnShellSimulationCollisionArgs e)
	{
		collisionMarker.enabled = true;
		this.transform.position = Camera.main.WorldToScreenPoint(e.collisionPosition);
	}
}
