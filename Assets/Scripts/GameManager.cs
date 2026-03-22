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

    [Header("Debug")]
    [SerializeField] private bool enablePlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (enablePlayer)
            playerShip.ChangeControllerToPlayer(playerShipController, playerTurretController);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<ShipHandler> GetTargetShips(ShipHandler caller)
    {
        if (teamA.Contains(caller))
            return teamB;
        else if (teamB.Contains(caller))
            return teamA;
        else
            throw new System.Exception("Ship not assigned to team: " + caller.ToString());
    }
}
