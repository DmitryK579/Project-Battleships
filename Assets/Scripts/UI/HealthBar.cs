using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [field: SerializeField] public ShipHandler TrackedShip {get; private set; }
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private bool disappearOnDeplete;

    float maxHealth;
    float currentHealth;
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth = TrackedShip.GetCurrentHealth();
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
        maxHealth = TrackedShip.Ship.Health;
    }
}
