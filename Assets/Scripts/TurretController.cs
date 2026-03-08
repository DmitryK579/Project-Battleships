using System;
using UnityEngine;

public abstract class TurretController : MonoBehaviour
{
	public event EventHandler OnShoot;

	protected virtual void InvokeOnShoot(EventArgs e)
	{
		this.OnShoot?.Invoke(this, e);
	}
	public abstract Vector3 GetTargetCoordinates();
}
