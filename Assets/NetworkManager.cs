using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	private const string LEVEL = "level";
	private const string MODE = "mode";
	private const string TEAM = "team";
	private const string IS_READY = "is_ready";
	private const byte MAX_PLAYERS = 2;

	[SerializeField] private ChessUIManager uiManager;
	[SerializeField] private GameInitializer gameInitializer;
	private MultiplayerChessGameController chessGameController;
	private MultiplayerClassicChessGameController chessClassicGameController;

	private ChessLevel playerLevel;
	private Mode playerMode;

	private bool is_waiting = false;
	private int waiting_time = 0;

	void Awake()
	{
		PhotonNetwork.AutomaticallySyncScene = true;
	}
	private void Update()
	{
		//Дебаг обновления статуса соединения - сейчас не работает
		//uiManager.SetConnectionStatusText(PhotonNetwork.NetworkClientState.ToString());
	}

	public void SetDependenciesChaos(MultiplayerChessGameController chessGameController)
	{
		this.chessGameController = chessGameController;
	}
	public void SetDependenciesClassic(MultiplayerClassicChessGameController chessGameController)
	{
		this.chessClassicGameController = chessGameController;
	}

	public void Connect()
	{
		if (PhotonNetwork.IsConnected)
		{
			PhotonNetwork.JoinRandomRoom(new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel } }, MAX_PLAYERS);
		}
		else
		{
			PhotonNetwork.ConnectUsingSettings();
		}
	}

	#region Photon Callbacks

	public override void OnConnectedToMaster()
	{
		
		Debug.LogError($"Connected to server. Looking for random room with level {playerLevel}");
		PhotonNetwork.JoinRandomRoom(new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel }, { MODE, playerMode } }, MAX_PLAYERS);
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.LogError($"Joining random room failed becuse of {message}. Creating new one with player level {playerLevel}");
		PhotonNetwork.CreateRoom(null, new RoomOptions
		{
			CustomRoomPropertiesForLobby = new string[] { LEVEL, MODE },
			MaxPlayers = MAX_PLAYERS,
			CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { LEVEL, playerLevel }, { MODE, playerMode } }
		});
	}

	public override void OnJoinedRoom()
	{
		Debug.LogError($"Player {PhotonNetwork.LocalPlayer.ActorNumber} joined a room with level: {(ChessLevel)PhotonNetwork.CurrentRoom.CustomProperties[LEVEL]}");
		
		if ((Mode)PhotonNetwork.CurrentRoom.CustomProperties[MODE] == (Mode)1)
			gameInitializer.CreateMultiplayerBoard();
		if ((Mode)PhotonNetwork.CurrentRoom.CustomProperties[MODE] == (Mode)0)
			gameInitializer.CreateMultiplayerClassicBoard();

		var screen = uiManager.ShowTeamSelectionScreen();
		if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
			uiManager.OnRoomReady();
		else
			uiManager.OnRoomPrepare();
		
		//Предпосылки ограниченного времени ожидания
		//while (PhotonNetwork.CurrentRoom.PlayerCount < 2) { is_waiting = true; };
	}

    public override void OnLeftRoom()
    {
		var self = PhotonNetwork.LocalPlayer;
		Debug.LogError("You left room");

		if (self.CustomProperties.ContainsKey(IS_READY))
		{
			PhotonNetwork.LocalPlayer.CustomProperties[IS_READY] = false;
			PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { IS_READY, false } });
		}

		if (self.CustomProperties.ContainsKey(MODE))
		{
			if ((Mode)self.CustomProperties[MODE] == (Mode)1)
			{
				if (chessGameController != null)
					chessGameController.RestartGame();
				else
					PhotonNetwork.Disconnect();

				if (self.CustomProperties.ContainsKey(TEAM))
					chessGameController.SetupCamera((TeamColor)PhotonNetwork.LocalPlayer.CustomProperties[TEAM]);
			}
			if ((Mode)self.CustomProperties[MODE] == (Mode)0)
			{
				if (chessClassicGameController != null)
					chessClassicGameController.RestartGame();
				else
					PhotonNetwork.Disconnect();

				if (self.CustomProperties.ContainsKey(TEAM))
					chessClassicGameController.SetupCamera((TeamColor)PhotonNetwork.LocalPlayer.CustomProperties[TEAM]);
			}
		}
		
		PhotonNetwork.RemovePlayerCustomProperties(null);
		uiManager.OnGameLaunched();
	}

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
		Debug.LogError($"Player {otherPlayer.ActorNumber} left room");
		PhotonNetwork.LeaveRoom();
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Debug.LogError($"Player {newPlayer.ActorNumber} entered a room");
		var screen = uiManager.ShowTeamSelectionScreen();
		if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
			uiManager.OnRoomReady();
		else
			uiManager.OnRoomPrepare();
		// Разблокировать кнопку "начать" если игроков макс
	}
    #endregion

    #region Players Settings for game
    public void SetPlayerLevel(ChessLevel level)
	{
		playerLevel = level;
		PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { LEVEL, level } });
		PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { IS_READY, false } });
	}
	public void SetPlayerMode(Mode mode)
	{
		playerMode = mode;
		PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { MODE, mode } });
		PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { IS_READY, false } });
	}

	public void SetPlayerTeam(int teamInt)
	{
		if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
		{
			var player1 = PhotonNetwork.CurrentRoom.GetPlayer(1);
			var player2 = PhotonNetwork.CurrentRoom.GetPlayer(2);
			if (player1.CustomProperties.ContainsKey(TEAM))
			{
				var occupiedTeam = player1.CustomProperties[TEAM];
				teamInt = (int)occupiedTeam == 0 ? 1 : 0;
			}
			if (player2.CustomProperties.ContainsKey(TEAM))
			{
				var occupiedTeam = player2.CustomProperties[TEAM];
				teamInt = (int)occupiedTeam == 0 ? 1 : 0;
			}
		}
		PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { TEAM, teamInt } });

		if ((Mode)PhotonNetwork.CurrentRoom.CustomProperties[MODE] == (Mode)1)
		{
			gameInitializer.InitializeMultiplayerController();
			chessGameController.SetupCamera((TeamColor)teamInt);
			chessGameController.SetLocalPlayer((TeamColor)teamInt);
		}

		if ((Mode)PhotonNetwork.CurrentRoom.CustomProperties[MODE] == (Mode)0)
		{
			gameInitializer.InitializeMultiplayerClassicController();
			chessClassicGameController.SetupCamera((TeamColor)teamInt);
			chessClassicGameController.SetLocalPlayer((TeamColor)teamInt);
		}

		PhotonNetwork.LocalPlayer.CustomProperties[IS_READY] = true;
		PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { IS_READY, true } });
		Debug.Log($"Local Player is {(bool)PhotonNetwork.LocalPlayer.CustomProperties[IS_READY]}");
		if ((Mode)PhotonNetwork.CurrentRoom.CustomProperties[MODE] == (Mode)1)
			chessGameController.StartNewGame();
		if ((Mode)PhotonNetwork.CurrentRoom.CustomProperties[MODE] == (Mode)0)
			chessClassicGameController.StartNewGame();
	}
    #endregion

    internal bool IsRoomFull()
	{
		return PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;
	}

	public void OnPlayerRestart()
    {
		if ((Mode)PhotonNetwork.CurrentRoom.CustomProperties[MODE] == (Mode)1)
			chessGameController.RestartGame();
		if ((Mode)PhotonNetwork.CurrentRoom.CustomProperties[MODE] == (Mode)0)
			chessClassicGameController.RestartGame();
	}

	public void OnPlayerWantBack()
    {
		PhotonNetwork.Disconnect();
	}

	public bool IsWinner(TeamColor winner)
    {
		if ((TeamColor)PhotonNetwork.LocalPlayer.CustomProperties[TEAM] == winner)
        {
			return true;
        }
		return false;
    }
}
