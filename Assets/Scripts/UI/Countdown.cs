using UnityEngine;
using TMPro;
using System;

public class Countdown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI battleBeginsMessage;
    [SerializeField] private TextMeshProUGUI countdown;

    private bool isEnabled = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		battleBeginsMessage.gameObject.SetActive(false);
		countdown.gameObject.SetActive(false);

		GameManager.Instance.OnCountdownBegin += OnGameCountdownBegin;
        GameManager.Instance.OnCountdownEnd += OnGameCountdownEnd;
    }

	private void OnDisable()
	{
		GameManager.Instance.OnCountdownBegin -= OnGameCountdownBegin;
		GameManager.Instance.OnCountdownEnd -= OnGameCountdownEnd;
	}

	private void OnGameCountdownBegin(object sender, EventArgs e)
	{
		isEnabled = true;
		battleBeginsMessage.gameObject.SetActive(true);
		countdown.gameObject.SetActive(true);
	}

	private void OnGameCountdownEnd(object sender, EventArgs e)
	{
		gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update()
    {
		if (!isEnabled)
			return;

		countdown.text = Mathf.Ceil(GameManager.Instance.GetCountdownTimer()).ToString();
    }
}
