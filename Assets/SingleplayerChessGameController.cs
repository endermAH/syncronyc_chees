using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SingleplayerChessGameController : ChessGameController
{
	protected override void SetGameState(GameState state)
	{
		this.state = state;
	}

	public override void TryToStartThisGame()
	{
		SetGameState(GameState.Play);
		//cameraSetup.SetupCamera(TeamColor.Black);
	}

	public override bool CanPerformMove()
	{
		if (!IsGameInProgress())
			return false;
		return true;
	}

	public override bool IsTeamTurnActive(TeamColor team)
	{
		return activePlayer.team == team;
	}

	public override bool IsTeamLocal(TeamColor color)
	{
		return  activePlayer.team == color;
	}

	public override bool CheckIfGameIsFinished()
	{

		Piece[] kingAttackingPieces = activePlayer.GetPieceAtackingOppositePiceOfType<King>();
		if (kingAttackingPieces.Length > 0)
		{
			ChessPlayer oppositePlayer = GetOpponentToPlayer(activePlayer);
			Piece attackedKing = oppositePlayer.GetPiecesOfType<King>().FirstOrDefault();
			oppositePlayer.RemoveMovesEnablingAttakOnPieceOfType<King>(activePlayer, attackedKing);

			int avaliableKingMoves = attackedKing.avaliableMoves.Count;
			if (avaliableKingMoves == 0)
			{
				bool canCoverKing = oppositePlayer.CanHidePieceFromAttack<King>(activePlayer);
				if (!canCoverKing)
					return true;
			}
		}
		return false;
	}

	public override void BackToTheMainMenu()
    {
		StartNewGame();
	}
	public override void LeaveAfterEnd() {}

	public override void EndGame(ChessUIManager UIManager)
	{
		SetGameState(GameState.Finished);
		UIManager.OnGameFinishedSingle((TeamColor)activePlayer.team);

		if (blackPlayer.GetPiecesOfType<King>().FirstOrDefault() == null)
		{
			UIManager.OnGameFinishedSingle((TeamColor)GetOpponentToPlayer(blackPlayer).team);
			Debug.Log("");
		}
		if (whitePlayer.GetPiecesOfType<King>().FirstOrDefault() == null/*GetOpponentToPlayer(activePlayer).GetPiecesOfType<King>().FirstOrDefault() == null*/)
		{
			UIManager.OnGameFinishedSingle((TeamColor)blackPlayer.team);
		}
	}
}
