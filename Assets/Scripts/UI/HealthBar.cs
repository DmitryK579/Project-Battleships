using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private ShipHandler trackedShip;
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Optional settings")]
	[SerializeField] private bool useGameManager;
	[SerializeField] private bool disappearOnDeplete;

    float maxHealth;
    float currentHealth;
    void Start()
    {
        if (useGameManager)
        {
			GameManager.Instance.OnPlayerShipSwap += OnPlayerShipSwap;
            trackedShip = GameManager.Instance.PlayerShip;
        }
        Initialize();
    }

	private void OnDisable()
	{
        if (useGameManager)
			GameManager.Instance.OnPlayerShipSwap -= OnPlayerShipSwap;
	}

	// Update is called once per frame
	void Update()
    {
        currentHealth = trackedShip.GetCurrentHealth();
        healthBar.fillAmount = (currentHealth / maxHealth);
        if (healthText.IsActive())
            healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();

        if (!disappearOnDeplete)
            return;

        if (currentHealth == 0)
            Destroy(this.gameObject);
    }

    void Initialize()
    {
        maxHealth = trackedShip.Ship.Health;
    }

	private void OnPlayerShipSwap(object sender, EventArgs e)
    {
        trackedShip = GameManager.Instance.PlayerShip;
        Initialize();
    }
}
