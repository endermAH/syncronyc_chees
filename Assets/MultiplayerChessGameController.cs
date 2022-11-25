using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultiplayerChessGameController : ChessGameController, IOnEventCallback
{
	private NetworkManager networkManager;
	private ChessPlayer localPlayer;

    public void SetNetworkManager(NetworkManager networkManager)
	{
		this.networkManager = networkManager;
	}

	private void OnEnable()
	{
		PhotonNetwork.AddCallbackTarget(this);
	}

	private void OnDisable()
	{
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	public void SetLocalPlayer(TeamColor team)
	{
		localPlayer = team == TeamColor.White ? whitePlayer : blackPlayer;
	}

	public bool IsLocalPlayersTurn()
	{
		return localPlayer == activePlayer;
	}

	public void OnEvent(EventData photonEvent)
	{
		byte eventCode = photonEvent.Code;
		if (eventCode == SET_GAME_STATE_EVENT_CODE)
		{
			object[] data = (object[])photonEvent.CustomData;
			GameState state = (GameState)data[0];
			this.state = state;
		}

	}

	protected override void SetGameState(GameState state)
	{
		object[] content = new object[] { (int)state };
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
		PhotonNetwork.RaiseEvent(SET_GAME_STATE_EVENT_CODE, content, raiseEventOptions, SendOptions.SendReliable);
	}

    #region Override mathods

    // Попытка начать игру - установить состояние Play
    public override void TryToStartThisGame()
	{
		// Если комната заполнена (игрока в комнате 2)
		if (networkManager.IsRoomFull())
		{
			Debug.Log($"First Player is {(bool)PhotonNetwork.CurrentRoom.GetPlayer(1).CustomProperties["is_ready"]}" +
			$" and second Player is {(bool)PhotonNetwork.CurrentRoom.GetPlayer(2).CustomProperties["is_ready"]}");

			// то мы проверяем готовность обоих игроков и переключаем состояние
			if ((bool)PhotonNetwork.CurrentRoom.GetPlayer(1).CustomProperties["is_ready"]
				&& (bool)PhotonNetwork.CurrentRoom.GetPlayer(2).CustomProperties["is_ready"])
			{
				SetGameState(GameState.Play);
				Debug.Log("Game State is Play");
			}
		}
	}

	public override bool CanPerformMove()
	{
		return IsGameInProgress();
	}

	public override bool IsTeamTurnActive(TeamColor team)
    {
        return true;
    }

	public override bool IsTeamLocal(TeamColor color)
    {
		return localPlayer.team == color;
    }

	public override bool CheckIfGameIsFinished()
	{
		if(activePlayer.GetPiecesOfType<King>().FirstOrDefault() == null)
        {
			return true;
		}
		if (GetOpponentToPlayer(activePlayer).GetPiecesOfType<King>().FirstOrDefault() == null)
		{
			return true;
		}
		return false;
	}

	public override void BackToTheMainMenu()
    {
		PhotonNetwork.Disconnect();
    }

	public override void LeaveAfterEnd()
    {
		PhotonNetwork.LeaveRoom();
    }

	public override void EndGame(ChessUIManager UIManager)
	{
		SetGameState(GameState.Finished);

		if (activePlayer.GetPiecesOfType<King>().FirstOrDefault() == null)
		{
			LeaveAfterEnd();
			UIManager.OnGameFinished((TeamColor)GetOpponentToPlayer(activePlayer).team);
		}
		if (GetOpponentToPlayer(activePlayer).GetPiecesOfType<King>().FirstOrDefault() == null)
		{
			LeaveAfterEnd();
			UIManager.OnGameFinished((TeamColor)activePlayer.team);
		}
	}
    #endregion
}


