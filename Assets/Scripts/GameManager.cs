using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Player objects")]
    [SerializeField] private PlayerShipController playerShipController;
    [SerializeField] private PlayerTurretController playerTurretController;
    [field: SerializeField] public ShipHandler PlayerShip { get; private set; }

    [Header("Teams")]
    [SerializeField] private List<ShipHandler> teamA;
    [SerializeField] private List<ShipHandler> teamB;

    [Header("Settings")]
    [SerializeField] private float timeToForceSwapAfterDestructionS;

    [Header("Debug")]
    [SerializeField] private bool enablePlayer;

    private int playerShipIndex = 0;
    private List<ShipHandler> playerTeam;

    private bool playerShipDestroyed = false;
    private float forceSwapAfterDestructionTimer = 0f;

    public event EventHandler OnPlayerShipSwap;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		PlayerInputContainer.Instance.playerInputActions.GameManager.Enable();
        PlayerInputContainer.Instance.playerInputActions.GameManager.SwapShip.performed += OnSwapShipPerformed;

        if (enablePlayer)
        {
            PlayerShip.ChangeControllerToPlayer(playerShipController, playerTurretController);
            PlayerShip.OnZeroHealth += OnPlayerShipDestroyed;
        }

        if (teamA.Contains(PlayerShip))
        {
            ChangeHealthBarColours(teamA, teamB);
            playerTeam = teamA;
        }
        else if (teamB.Contains(PlayerShip))
        {
            ChangeHealthBarColours(teamB, teamA);
            playerTeam = teamB;
		}
    }

	private void OnDisable() 
	{
		PlayerInputContainer.Instance.playerInputActions.GameManager.Disable();
		PlayerInputContainer.Instance.playerInputActions.GameManager.SwapShip.performed -= OnSwapShipPerformed;

        if (PlayerShip != null)
        {
            PlayerShip.OnZeroHealth -= OnPlayerShipDestroyed;

		}
	}

	// Update is called once per frame
	void Update()
    {
        if (playerShipDestroyed)
        {
            forceSwapAfterDestructionTimer -= Time.deltaTime;
            if (forceSwapAfterDestructionTimer <= 0)
            {
                SwapPlayerShip();
            }
        }
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
        SwapPlayerShip();
	}

    private void SwapPlayerShip()
    {
		int nextIndex = GetNextValidShipIndex();
		if (nextIndex == playerShipIndex)
			return;

		if (playerShipDestroyed)
			playerShipDestroyed = false;

		PlayerShip.ResetControllerToOwnCPU();

        PlayerShip = playerTeam[nextIndex];
		PlayerShip.ChangeControllerToPlayer(playerShipController, playerTurretController);
		OnPlayerShipSwap?.Invoke(this, EventArgs.Empty);

		playerShipIndex = nextIndex;
	}

	private int GetNextValidShipIndex()
    {
        int initialIndex = playerShipIndex;
        int test = initialIndex;
        while (true)
        {
            test++;
            if (test > (playerTeam.Count - 1))
                test = 0;

            if (playerTeam[test] != null)
                break;

            if (test == initialIndex)
                break;
        }
        return test;
    }

    private void OnPlayerShipDestroyed(object sender, EventArgs e)
    {
        PlayerShip.OnZeroHealth -= OnPlayerShipDestroyed;
        playerShipDestroyed = true;
        forceSwapAfterDestructionTimer = timeToForceSwapAfterDestructionS;
    }
}
