using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class ShipStatus : MonoBehaviour
{
    [SerializeField] private Transform hullIcon;
    [SerializeField] private List<Transform> turretIcons;
    private List<TurretHandler> controllableTurrets;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		GameManager.Instance.OnPlayerShipSwap += OnPlayerShipSwap;
		Initialize();
    }

	private void OnDisable()
	{
		GameManager.Instance.OnPlayerShipSwap -= OnPlayerShipSwap;
	}

	// Update is called once per frame
	void Update()
    {
        if (GameManager.Instance.PlayerShip == null)
            return;
        
        hullIcon.transform.rotation = GameManager.Instance.PlayerShip.transform.rotation;
        for (int i = 0; i < turretIcons.Count; i++)
        {
            turretIcons[i].transform.rotation = controllableTurrets[i].transform.rotation;
        }
    }

    private void Initialize()
    {
		controllableTurrets = GameManager.Instance.PlayerShip.GetControllableTurrets();
	}

	private void OnPlayerShipSwap(object sender, EventArgs e)
	{
		Initialize();
	}
}
