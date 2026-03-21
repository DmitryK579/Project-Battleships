using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerShipController playerShipController;
    [SerializeField] private PlayerTurretController playerTurretController;
    [SerializeField] private ShipHandler playerShip;

    [SerializeField] private List<ShipHandler> teamA;
    [SerializeField] private List<ShipHandler> teamB;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
