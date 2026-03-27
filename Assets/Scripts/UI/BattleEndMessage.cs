using System;
using TMPro;
using UnityEngine;

public class BattleEndMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winMessage;
    [SerializeField] private TextMeshProUGUI loseMessage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winMessage.gameObject.SetActive(false);
        loseMessage.gameObject.SetActive(false);

        GameManager.Instance.OnBattleEnd += OnBattleEnd;
    }

	private void OnDisable()
	{
		GameManager.Instance.OnBattleEnd -= OnBattleEnd;
	}

	private void OnBattleEnd(object sender, EventArgs e)
	{
        if (GameManager.Instance.GetPlayerWon())
            winMessage.gameObject.SetActive(true);
        else
            loseMessage.gameObject.SetActive(true);
	}
}
