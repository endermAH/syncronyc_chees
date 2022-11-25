using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MaterialSetter))]
[RequireComponent(typeof(IObjectTweener))]
public abstract class Piece : MonoBehaviour
{
	[SerializeField] private MaterialSetter materialSetter;
	public Board board { protected get; set; }
	public Vector2Int occupiedSquare { get; set; }
	public TeamColor team { get; set; }
	public bool hasMoved { get; private set; }
	public List<Vector2Int> avaliableMoves;

	private IObjectTweener tweener;

	public abstract List<Vector2Int> SelectAvaliableSquares();
	public abstract void SetSelfMaterial();

	private void Awake()
	{
		avaliableMoves = new List<Vector2Int>();
		tweener = GetComponent<IObjectTweener>();
		materialSetter = GetComponent<MaterialSetter>();
		hasMoved = false;
	}

	public void SetMaterial(Material selectedMaterial)
	{
		materialSetter.SetSkinnedMaterial(selectedMaterial);
		transform.GetChild(0).transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material = selectedMaterial;
	}

	public void SetData(Vector2Int coords, TeamColor team, Board board)
	{
		this.team = team;
		occupiedSquare = coords;
		this.board = board;
		transform.position = board.CalculatePositionFromCoords(coords);
	}

	public void SetRotateOnBoard(TeamColor team)
	{
		if (team == TeamColor.White)
			transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
		else
			transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y + 180, transform.rotation.z, transform.rotation.w);
	}

	public bool IsFromSameTeam(Piece piece)
	{
		return team == piece.team;
	}

	public bool CanMoveTo(Vector2Int coords)
	{
		return avaliableMoves.Contains(coords);
	}

	public virtual void MovePiece(Vector2Int coords)
	{
		Debug.Log($"<Move piece> in <Piece> from {(TeamColor)team}");
		Vector3 targetPosition = board.CalculatePositionFromCoords(coords);
		occupiedSquare = coords;
		hasMoved = true;
		tweener.MoveTo(transform, targetPosition);
	}

	protected void TryToAddMove(Vector2Int coords)
	{
		avaliableMoves.Add(coords);
	}

	public bool IsAttackingPieceOfType<T>() where T : Piece
	{
		foreach (var square in avaliableMoves)
		{
			if (board.GetPieceOnSquare(square) is T)
				return true;
		}
		return false;
	}
}
