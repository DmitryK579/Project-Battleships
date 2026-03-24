using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretStatusCircle : MonoBehaviour
{
    [field: SerializeField] public TurretHandler TrackedTurret { get; set; }
	[SerializeField] private Transform shellBlockedIndicator;
    private Image progressImage;
	private bool reloading = false;
	private bool facingTarget = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
		TrackedTurret.OnShoot += OnTurretShoot;
		TrackedTurret.OnFacingTarget += OnTurretFacingTarget;
		TrackedTurret.OnNoLongerFacingTarget += OnTurretNoLongerFacingTarget;
		TrackedTurret.OnShellSimulationCollision += OnTurretShellSimulationCollision;
		TrackedTurret.OnShellSimulationPass += OnTurretShellSimulationPass;
		progressImage = GetComponent<Image>();
		ChangeImageColour();
	}

	private void OnDisable()
	{
		TrackedTurret.OnShoot -= OnTurretShoot;
		TrackedTurret.OnFacingTarget -= OnTurretFacingTarget;
		TrackedTurret.OnNoLongerFacingTarget -= OnTurretNoLongerFacingTarget;
		TrackedTurret.OnShellSimulationCollision -= OnTurretShellSimulationCollision;
		TrackedTurret.OnShellSimulationPass -= OnTurretShellSimulationPass;
	}

	// Update is called once per frame
	private void Update()
    {
		if (reloading)
		{
			if (TrackedTurret == null)
				return;

			(float reloadTime, float reloadTimer) = TrackedTurret.GetReloadTimeAndTimer();
			progressImage.fillAmount = (reloadTime - reloadTimer) / reloadTime;
			if (progressImage.fillAmount == 1)
			{
				reloading = false;
				ChangeImageColour();
			}
		}
    }

	private void OnTurretShoot(object sender, EventArgs e)
	{
		reloading = true;
		ChangeImageColour();
	}

	private void OnTurretFacingTarget(object sender, EventArgs e)
	{
		facingTarget = true;
		ChangeImageColour();
	}

	private void OnTurretNoLongerFacingTarget(object sender, EventArgs e)
	{
		facingTarget = false;
		ChangeImageColour();
	}

	private void ChangeImageColour()
	{
		progressImage.color = Color.green;

		if (reloading)
			progressImage.color = Color.greenYellow;

		if (!facingTarget)
			progressImage.color = Color.orange;
	}

	private void OnTurretShellSimulationCollision(object sender, TurretHandler.OnShellSimulationCollisionArgs e)
	{
		shellBlockedIndicator.gameObject.SetActive(true);
	}

	private void OnTurretShellSimulationPass(object sender, EventArgs e)
	{
		shellBlockedIndicator.gameObject.SetActive(false);
	}
}
