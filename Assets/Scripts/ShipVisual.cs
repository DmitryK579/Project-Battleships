using System;
using UnityEngine;

public class ShipVisual : MonoBehaviour
{
	[SerializeField] private Transform explosionPrefab;
    private SpriteRenderer shipSprite;
    private ShipHandler parentObjectScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shipSprite = GetComponent<SpriteRenderer>();
		parentObjectScript = transform.parent.GetComponent<ShipHandler>();
        parentObjectScript.OnCriticallyDamaged += OnCriticallyDamaged;
        parentObjectScript.OnZeroHealth += OnZeroHealth;
	}

	private void OnDisable()
	{
		parentObjectScript.OnCriticallyDamaged -= OnCriticallyDamaged;
		parentObjectScript.OnZeroHealth -= OnZeroHealth;
	}
	private void OnCriticallyDamaged(object sender, EventArgs e)
    {
		shipSprite.color = Color.lightGray;
    }

	private void OnZeroHealth(object sender, EventArgs e)
	{
		Instantiate(explosionPrefab, this.transform.position, this.transform.rotation);
	}
}
