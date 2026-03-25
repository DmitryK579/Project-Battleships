using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class ShipStatus : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform hullIcon;
    [SerializeField] private List<Transform> turretIcons;
    private List<TurretHandler> controllableTurrets;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		gameManager.OnPlayerShipSwap += OnPlayerShipSwap;
		Initialize();
    }

	private void OnDisable()
	{
        gameManager.OnPlayerShipSwap -= OnPlayerShipSwap;
	}

	// Update is called once per frame
	void Update()
    {
        if (gameManager.PlayerShip == null)
            return;
        
        hullIcon.transform.rotation = gameManager.PlayerShip.transform.rotation;
        for (int i = 0; i < turretIcons.Count; i++)
        {
            turretIcons[i].transform.rotation = controllableTurrets[i].transform.rotation;
        }
    }

    private void Initialize()
    {
		controllableTurrets = gameManager.PlayerShip.GetControllableTurrets();
	}

	private void OnPlayerShipSwap(object sender, EventArgs e)
	{
		Initialize();
	}
}
