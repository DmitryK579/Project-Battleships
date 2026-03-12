using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretStatusCircle : MonoBehaviour
{
    [field: SerializeField] public TurretHandler TrackedTurret { get; set; }
	[SerializeField] private Transform collisionMark;
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
	}

	// Update is called once per frame
	private void Update()
    {
		if (reloading)
		{
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
			progressImage.color = Color.red;

		if (!facingTarget)
			progressImage.color = Color.orange;
	}

	private void OnTurretShellSimulationPass(object sender, EventArgs e)
	{
		collisionMark.gameObject.SetActive(false);
	}

	private void OnTurretShellSimulationCollision(object sender, TurretHandler.OnShellSimulationCollisionArgs e)
	{
		collisionMark.gameObject.SetActive(true);
		collisionMark.transform.position = Camera.main.WorldToScreenPoint(e.collisionPosition);
	}
}
