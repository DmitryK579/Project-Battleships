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

    public enum GameState
    {
        PreCountdown,
        Countdown,
        Playing,
        Paused,
        BattleEnd
    }

    public static GameManager Instance { get; private set; } 

    private GameState gameState = GameState.PreCountdown;

    private class GameStateTimerHandler
    {
        private float preCountdownTimeS = 1f;
		private float countdownTimeS = 5f;
		private float battleEndTimeS = 10f;

        public float Timer { get; private set; } = 0f;

        public void SetTimerByGameState(GameState state)
        {
            switch (state)
            {
                case GameState.PreCountdown:
                    {
                        Timer = preCountdownTimeS;
                        break;
                    }
                case GameState.Countdown:
                    {
						Timer = countdownTimeS;
                        break;
                    }
                case GameState.Playing:
                    {
                        Timer = 0f;
                        break;
                    }
                case GameState.BattleEnd:
                    {
						Timer = battleEndTimeS;
                        break;
                    }
            }
        }

        public void TickDown(float time)
        {
            Timer -= time;
            if (Timer < 0f)
                Timer = 0f;
        }
    }

    private GameStateTimerHandler gameStateTimerHandler = new GameStateTimerHandler();

    private int playerShipIndex = 0;
    private List<ShipHandler> playerTeam;

    private bool playerShipDestroyed = false;
    private float forceSwapAfterDestructionTimer = 0f;

    private int teamADestroyed = 0;
    private int teamBDestroyed = 0;
    private bool playerTeamWon = false;
    private bool isPaused = false;

    public event EventHandler OnPlayerShipSwap;
    public event EventHandler OnCountdownBegin;
    public event EventHandler OnCountdownEnd;
    public event EventHandler OnBattleEnd;
    public event EventHandler OnPause;
    public event EventHandler OnUnPause;

	private void Awake()
	{
		Instance = this;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		PlayerInputContainer.Instance.playerInputActions.GameManager.Enable();
        PlayerInputContainer.Instance.playerInputActions.GameManager.SwapShip.performed += OnSwapShipPerformed;
        PlayerInputContainer.Instance.playerInputActions.GameManager.Pause.performed += OnPausePerformed;

        if (enablePlayer)
        {
            PlayerShip.ChangeControllerToPlayer(playerShipController, playerTurretController);
            SubscribeToPlayerShipEvents();
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

        foreach(ShipHandler ship in teamA)
        {
            ship.OnZeroHealth += OnTeamAShipDestroyed;
        }
		foreach (ShipHandler ship in teamB)
		{
			ship.OnZeroHealth += OnTeamBShipDestroyed;
		}

		gameState = GameState.PreCountdown;
        gameStateTimerHandler.SetTimerByGameState(gameState);
    }

	private void OnDisable() 
	{
		PlayerInputContainer.Instance.playerInputActions.GameManager.Disable();
		PlayerInputContainer.Instance.playerInputActions.GameManager.SwapShip.performed -= OnSwapShipPerformed;
		PlayerInputContainer.Instance.playerInputActions.GameManager.Pause.performed -= OnPausePerformed;

        if (PlayerShip != null)
        {
            UnsubscribeFromPlayerShipEvents();
		}
	}

    // Update is called once per frame
    void Update()
    {
        SwapIfPlayerShipDestroyed();

        if (gameState != GameState.BattleEnd)
        {
            if (teamADestroyed == teamA.Count)
            {
                BattleEnd(teamA);
            }

            if (teamBDestroyed == teamB.Count)
            {
                BattleEnd(teamB);
            }
        }

        if (gameState == GameState.Playing)
            return;

        gameStateTimerHandler.TickDown(Time.deltaTime);
        if (gameStateTimerHandler.Timer == 0f)
        {
            switch (gameState)
            {
                case GameState.PreCountdown:
                    {
                        gameState = GameState.Countdown;
                        OnCountdownBegin?.Invoke(this, EventArgs.Empty);
                        break;
                    }
                case GameState.Countdown:
                    {
                        gameState = GameState.Playing;
                        OnCountdownEnd?.Invoke(this, EventArgs.Empty);
                        break;
                    }
                case GameState.BattleEnd:
                    {
                        Loader.Load(Loader.Scene.MainMenuScene);
                        break;
                    }
            }
            gameStateTimerHandler.SetTimerByGameState(gameState);
        }
    }

    private void BattleEnd(List<ShipHandler> team)
    {
        if (team == playerTeam)
            playerTeamWon = false;
        else
            playerTeamWon = true;

		gameState = GameState.BattleEnd;
        gameStateTimerHandler.SetTimerByGameState(gameState);
        OnBattleEnd?.Invoke(this, EventArgs.Empty);
	}
    private void SwapIfPlayerShipDestroyed()
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

	private void OnPausePerformed(InputAction.CallbackContext context)
	{
		if (!isPaused)
            PauseGame();
        else
            UnPauseGame();
	}

    private void PauseGame()
    {
		Time.timeScale = 0f;
		isPaused = true;
		OnPause?.Invoke(this, EventArgs.Empty);
	}

    private void UnPauseGame()
    {
		Time.timeScale = 1f;
		isPaused = false;
		OnUnPause?.Invoke(this, EventArgs.Empty);
	}

    public void ForceUnPause()
    {
        UnPauseGame();
    }

	private void SwapPlayerShip()
    {
		int nextIndex = GetNextValidShipIndex();
		if (nextIndex == playerShipIndex)
			return;

		if (playerShipDestroyed)
			playerShipDestroyed = false;
        
        PlayerShip.ResetControllerToOwnCPU();
        UnsubscribeFromPlayerShipEvents();

        PlayerShip = playerTeam[nextIndex];
        PlayerShip.ChangeControllerToPlayer(playerShipController, playerTurretController);
        SubscribeToPlayerShipEvents();
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
        UnsubscribeFromPlayerShipEvents();
        playerShipDestroyed = true;
        forceSwapAfterDestructionTimer = timeToForceSwapAfterDestructionS;
    }

	private void OnTeamAShipDestroyed(object sender, EventArgs e)
	{
		if (sender is ShipHandler ship)
        {
            ship.OnZeroHealth -= OnTeamAShipDestroyed;
            teamADestroyed++;
            return;
        }
	}
	private void OnTeamBShipDestroyed(object sender, EventArgs e)
	{
		if (sender is ShipHandler ship)
		{
			ship.OnZeroHealth -= OnTeamBShipDestroyed;
			teamBDestroyed++;
            return;
		}
	}

	private void UnsubscribeFromPlayerShipEvents()
    {
		PlayerShip.OnZeroHealth -= OnPlayerShipDestroyed;
	}

	private void SubscribeToPlayerShipEvents()
    {
		PlayerShip.OnZeroHealth += OnPlayerShipDestroyed;
	}

    public GameState GetGameState()
    {
        return gameState;
    }

    public float GetCountdownTimer()
    {
        if (gameState == GameState.Countdown)
            return gameStateTimerHandler.Timer;
        return 0;
    }

    public bool GetPlayerWon()
    {
        return playerTeamWon;
    }
}
