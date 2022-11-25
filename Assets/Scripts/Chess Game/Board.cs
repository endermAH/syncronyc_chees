using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SquareSelectorCreator))]
public abstract class Board : MonoBehaviour
{
    public const int BOARD_SIZE = 8;

    protected const byte MOVE_PIECE_ON_POSITION = 9;

    [SerializeField] private Transform bottomLeftSquareTransform;
    [SerializeField] private float squareSize;

	public Piece[,] grid;
    public Piece selectedPiece;

    private Piece selectedPieceWhite;
    private Piece selectedPieceBlack;

    private ChessGameController chessController;
    private SquareSelectorCreator squareSelector;

    protected virtual void Awake()
    {
        squareSelector = GetComponent<SquareSelectorCreator>();
        CreateGrid();
    }

    public void SetDependencies(ChessGameController chessController)
    {
        this.chessController = chessController;
    }

    #region Work with coords of field

    private void CreateGrid()
    {
        grid = new Piece[BOARD_SIZE, BOARD_SIZE];
    }

    public Vector3 CalculatePositionFromCoords(Vector2Int coords)
    {
        return bottomLeftSquareTransform.position + new Vector3(coords.x * squareSize, 0f, coords.y * squareSize);
    }
    private Vector2Int CalculateCoordsFromPosition(Vector3 inputPosition)
    {
        int x = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).x / squareSize) + BOARD_SIZE / 2;
        int y = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).z / squareSize) + BOARD_SIZE / 2;
        return new Vector2Int(x, y);
    }
    public void UpdateBoardOnPieceMove(Vector2Int newCoords, Vector2Int oldCoords, Piece newPiece, Piece oldPiece)
    {
        grid[oldCoords.x, oldCoords.y] = oldPiece;
        grid[newCoords.x, newCoords.y] = newPiece;
    }

    public Piece GetPieceOnSquare(Vector2Int coords)
    {
        if (CheckIfCoordinatesAreOnBoard(coords))
            return grid[coords.x, coords.y];
        return null;
    }

    public bool CheckIfCoordinatesAreOnBoard(Vector2Int coords)
    {
        if (coords.x < 0 || coords.y < 0 || coords.x >= BOARD_SIZE || coords.y >= BOARD_SIZE)
            return false;
        return true;
    }

    public bool HasPiece(Piece piece)
    {
        for (int i = 0; i < BOARD_SIZE; i++)
        {
            for (int j = 0; j < BOARD_SIZE; j++)
            {
                if (grid[i, j] == piece)
                    return true;
            }
        }
        return false;
    }

    public void SetPieceOnBoard(Vector2Int coords, Piece piece)
    {
        if (CheckIfCoordinatesAreOnBoard(coords))
            grid[coords.x, coords.y] = piece;
    }
    #endregion

    #region Abstract methods
    public abstract void SelectedPieceMoved(Vector2 coords, Piece piece);
    public abstract void SetSelectedPiece(Vector2 coords);
    #endregion

    internal void OnSetSelectedPiece(Vector2Int coords)
    {
        Piece piece = GetPieceOnSquare(coords);

        if (chessController.IsTeamLocal(TeamColor.White) && piece.team == TeamColor.White)
        { 
            selectedPieceWhite = piece;
            Debug.Log($"<On set selected piece> in <Board>. Selected White piece is {piece} from {(TeamColor)piece.team}");
        }
        else if (chessController.IsTeamLocal(TeamColor.Black) && piece.team == TeamColor.Black)
        {
            selectedPieceBlack = piece;
            Debug.Log($"<On set selected piece> in <Board>. Selected Black piece is {piece} from {(TeamColor)piece.team}"); 
        }
    }

    // ПЕРЕДВИГАЕМ ВЫБРАННУЮ ПЕШКУ
    internal void OnSelectedPieceMoved(Vector2Int intCoords, Vector2Int pieceCoords)//
    {
        // убираем срубленные, если требуется
        TryToTakeOppositePiece(intCoords);

        Piece piece = GetPieceOnSquare(pieceCoords);

        // двигаем
        UpdateBoardOnPieceMove(intCoords, piece.occupiedSquare, piece, null);
        piece.MovePiece(intCoords);
        Debug.Log($"<On selected piece moved> in <Board>. Selected piece is {piece} from {(TeamColor)piece.team} sholud be moved and deselect");

        // если двигалась наша - снимаем выбор с неё
        if (chessController.IsTeamLocal(piece.team))
            DeselectPiece();

        EndTurn();
    }

    // ВЫБОР КЛЕТКИ
    public void OnSquareSelected(Vector3 inputPosition)
    {
        Debug.Log($"<On square selected> in <Board>");

        if (!chessController.IsGameInProgress())
            return;

        Vector2Int coords = CalculateCoordsFromPosition(inputPosition);
        Piece piece = GetPieceOnSquare(coords);
        Debug.Log($"<On square selected> in <Board>. This piece is {piece}.");
        
        if (chessController.IsTeamLocal(TeamColor.White))
        {
            // Если пешка уже выбрана
            if (selectedPieceWhite)
            {
                // Если мы выбираем ту же пешку снова
                if (piece != null && selectedPieceWhite == piece && chessController.IsTeamLocal(piece.team))
                {
                    DeselectPiece();
                }
                // Если мы решили изменить выбор пешки
                else if (piece != null && selectedPieceWhite != piece && chessController.IsTeamLocal(piece.team) && chessController.IsTeamTurnActive(piece.team))
                {
                    // опускаем предыдущую
                    var pTrans = selectedPieceWhite.transform;
                    selectedPieceWhite.transform.SetPositionAndRotation(new Vector3(pTrans.position.x, pTrans.position.y - (float)0.5, pTrans.position.z), pTrans.rotation);
                    
                    SelectPiece(coords);
                }
                // Если пешка уже выбрана, и мы ткнули на квадрат, в который она может сходить
                else if (selectedPieceWhite.CanMoveTo(coords))
                {
                    selectedPiece = selectedPieceWhite;
                    SelectedPieceMoved(coords, selectedPieceWhite);
                }
            }
            // Если пешка ещё не была выбрана
            else
            {
                if (piece != null && chessController.IsTeamLocal(piece.team) && chessController.IsTeamTurnActive(piece.team))
                {
                    SelectPiece(coords);
                }
            }
        }
        else
        {
            if (selectedPieceBlack)
            {
                if (piece != null && selectedPieceBlack == piece && chessController.IsTeamLocal(piece.team))
                {
                    DeselectPiece();
                }
                else if (piece != null && selectedPieceBlack != piece && chessController.IsTeamLocal(piece.team) && chessController.IsTeamTurnActive(piece.team))
                {
                    var pTrans = selectedPieceBlack.transform;
                    selectedPieceBlack.transform.SetPositionAndRotation(new Vector3(pTrans.position.x, pTrans.position.y - (float)0.5, pTrans.position.z), pTrans.rotation);

                    SelectPiece(coords);
                }
                else if (selectedPieceBlack.CanMoveTo(coords))
                {
                    selectedPiece = selectedPieceBlack;
                    SelectedPieceMoved(coords, selectedPieceBlack);
                }
            }
            else
            {
                if (piece != null && chessController.IsTeamLocal(piece.team) && chessController.IsTeamTurnActive(piece.team))
                {
                    SelectPiece(coords);
                }
                    
            }
        }
    }

    private void SelectPiece(Vector2Int coords)
    {
        Piece piece = GetPieceOnSquare(coords);

        List<Vector2Int> selection = piece.avaliableMoves;
        chessController.RemoveMovesEnablingAttakOnPieceOfType<King>(piece);
        SetSelectedPiece(coords); // - abstract

        var pTrans = piece.transform;
        piece.transform.SetPositionAndRotation(new Vector3(pTrans.position.x, pTrans.position.y + (float)0.5, pTrans.position.z), pTrans.rotation);

        Debug.Log($"<Select piece> in <Board>. We select {piece} from {(TeamColor)piece.team}.");

        if (chessController.IsTeamLocal(TeamColor.Black))
        {
            selection = selectedPieceBlack.avaliableMoves; 
        }
        else
        {
            selection = selectedPieceWhite.avaliableMoves;
        }
        ShowSelectionSquares(selection);
    }

    private void ShowSelectionSquares(List<Vector2Int> selection)
    {
        Dictionary<Vector3, bool> squaresData = new Dictionary<Vector3, bool>();
        for (int i = 0; i < selection.Count; i++)
        {
            Vector3 position = CalculatePositionFromCoords(selection[i]);
            bool isSquareFree = GetPieceOnSquare(selection[i]) == null;
            squaresData.Add(position, isSquareFree);
        }
        squareSelector.ShowSelection(squaresData);
    }

    private void DeselectPiece()
    {
        Transform pTrans;

        if (chessController.IsTeamLocal(TeamColor.White))
        {
            pTrans = selectedPieceWhite.transform;
            selectedPieceWhite.transform.SetPositionAndRotation(new Vector3(pTrans.position.x, pTrans.position.y - (float)0.5, pTrans.position.z), pTrans.rotation);
            Debug.Log($"<Deselect piece> in <Board> for white player. Down {selectedPieceWhite}. Black select is {selectedPieceBlack} and white is {selectedPieceWhite}. Your selected piece is {selectedPiece}");
            selectedPieceWhite = null;
        }
        else
        {
            pTrans = selectedPieceBlack.transform;
            selectedPieceBlack.transform.SetPositionAndRotation(new Vector3(pTrans.position.x, pTrans.position.y - (float)0.5, pTrans.position.z), pTrans.rotation);
            Debug.Log($"<Deselect piece> in <Board> for black player. Down {selectedPieceBlack}. Black select is {selectedPieceBlack} and white is {selectedPieceWhite}. Your selected piece is {selectedPiece}");
            selectedPieceBlack = null;
        }
        squareSelector.ClearSelection();
    }

    private void EndTurn()
    {
        chessController.EndTurn();
    }

    private void TryToTakeOppositePiece(Vector2Int coords)
    {
        Piece piece = GetPieceOnSquare(coords);
        if (piece)
        {
            {
                Debug.LogError("I try to take enemy!");
                TakePiece(piece);
            }
        }
    }

    private void TakePiece(Piece piece)
    {
        if (piece)
        {
            Debug.LogError("I should already take or I change");
            grid[piece.occupiedSquare.x, piece.occupiedSquare.y] = null;
            chessController.OnPieceRemoved(piece);
            if (piece == selectedPieceWhite || piece == selectedPieceBlack)
                squareSelector.ClearSelection();
            Destroy(piece.gameObject);
        }
    }


    public void PromotePiece(Piece piece)
    {
        TakePiece(piece);
        chessController.CreatePieceAndInitialize(piece.occupiedSquare, piece.team, typeof(Queen));
    }

    internal void OnGameRestarted()
    {
        squareSelector.ClearSelection();
        selectedPiece = null;
        selectedPieceBlack = null;
        selectedPieceWhite = null;
        CreateGrid();
    }
}
