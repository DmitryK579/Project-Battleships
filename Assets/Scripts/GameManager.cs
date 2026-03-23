using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerShipController playerShipController;
    [SerializeField] private PlayerTurretController playerTurretController;
    [SerializeField] private ShipHandler playerShip;

    [SerializeField] private List<ShipHandler> teamA;
    [SerializeField] private List<ShipHandler> teamB;

    [Header("Debug")]
    [SerializeField] private bool enablePlayer;

    private int playerShipIndex = 0;
    private List<ShipHandler> playerTeam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		PlayerInputContainer.Instance.playerInputActions.GameManager.Enable();
        PlayerInputContainer.Instance.playerInputActions.GameManager.SwapShip.performed += OnSwapShipPerformed;

		if (enablePlayer)
            playerShip.ChangeControllerToPlayer(playerShipController, playerTurretController);

        if (teamA.Contains(playerShip))
        {
            ChangeHealthBarColours(teamA, teamB);
            playerTeam = teamA;
        }
        else if (teamB.Contains(playerShip))
        {
            ChangeHealthBarColours(teamB, teamA);
            playerTeam = teamB;
		}
    }

	private void OnDisable()
	{
		PlayerInputContainer.Instance.playerInputActions.GameManager.Disable();
		PlayerInputContainer.Instance.playerInputActions.GameManager.SwapShip.performed -= OnSwapShipPerformed;
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

    public void ChangeHealthBarColours(List<ShipHandler> alliedTeam, List<ShipHandler> opposingTeam)
    {
        foreach (ShipHandler ship in alliedTeam)
        {
            ship.ChangeHealthBarColor(Color.aliceBlue);
        }

        foreach (ShipHandler ship in opposingTeam)
        {
            ship.ChangeHealthBarColor(Color.orangeRed);
        }
    }

    private void OnSwapShipPerformed(InputAction.CallbackContext context)
    {
        playerTeam[playerShipIndex].ResetControllerToOwnCPU();
        playerShipIndex++;
        if (playerShipIndex > (playerTeam.Count-1))
            playerShipIndex = 0;
        playerTeam[playerShipIndex].ChangeControllerToPlayer(playerShipController, playerTurretController);
    }
}
