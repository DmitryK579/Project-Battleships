using System;
using UnityEngine;

public class ShellVisual : MonoBehaviour
{
	[SerializeField] private GameObject splashParticlesPrefab;
	[SerializeField] private GameObject explosionParticlesPrefab;
	private Shell parentObjectScript;
	private bool collided = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        parentObjectScript = transform.parent.GetComponent<Shell>();
        parentObjectScript.OnReachedTarget += Hide;
        parentObjectScript.OnReachedTarget += CreateSplash;
        parentObjectScript.OnHit += CreateExplosion;
    }

	private void Hide(object sender, EventArgs e)
	{
	    this.gameObject.SetActive(false);
	}

	private void CreateSplash(object sender, EventArgs e)
	{
		if (!collided)
		{
			Instantiate(splashParticlesPrefab, this.transform.position, this.transform.rotation);
		}
	}

	private void CreateExplosion(object sender, EventArgs e)
	{
		Instantiate(explosionParticlesPrefab, this.transform.position, this.transform.rotation);
		collided = true;
	}
}
