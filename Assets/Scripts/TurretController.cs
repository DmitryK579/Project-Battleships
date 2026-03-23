using System;
using UnityEngine;

public abstract class TurretController : MonoBehaviour
{
	public event EventHandler OnShoot;
	public event EventHandler OnIdle;
	public event EventHandler OnNoLongerIdle;

	protected virtual void InvokeOnShoot(EventArgs e)
	{
		this.OnShoot?.Invoke(this, e);
	}
	public void SetTurretToIdle()
	{
		this.OnIdle?.Invoke(this, EventArgs.Empty);
	}
	public void SetTurretToNoLongerIdle()
	{
		this.OnNoLongerIdle?.Invoke(this, EventArgs.Empty);
	}
	public abstract Vector3 GetTargetCoordinates();
}
