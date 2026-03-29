using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
	[SerializeField] private Button continueButton;
	[SerializeField] private Button quitButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.OnPause += OnPause;
        GameManager.Instance.OnUnPause += OnUnPause;

		continueButton.onClick.AddListener(() =>
		{
			GameManager.Instance.ForceUnPause();
		});
		quitButton.onClick.AddListener(() =>
		{
			GameManager.Instance.ForceUnPause();
			Loader.Load(Loader.Scene.MainMenuScene);
		});
	}

	private void OnDisable()
	{
		GameManager.Instance.OnPause -= OnPause;
		GameManager.Instance.OnUnPause -= OnUnPause;
	}

	private void OnPause(object sender, EventArgs e)
	{
		EnablePauseScreen();
	}

	private void OnUnPause(object sender, EventArgs e)
	{
		DisablePauseScreen();
	}

	private void EnablePauseScreen()
	{
		foreach (Transform child in transform)
			child.gameObject.SetActive(true);
	}

	private void DisablePauseScreen()
	{
		foreach (Transform child in transform)
			child.gameObject.SetActive(false);
	}
}
