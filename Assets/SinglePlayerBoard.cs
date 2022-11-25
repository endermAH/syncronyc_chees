using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerBoard : Board
{
    public override void SelectedPieceMoved(Vector2 coords, Piece piece)
    {
        Vector2Int piece_coords = new Vector2Int(piece.occupiedSquare.x, piece.occupiedSquare.y);
        Vector2Int intCoords = new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
        OnSelectedPieceMoved(intCoords, piece_coords);
    }

    public override void SetSelectedPiece(Vector2 coords)
    {
        Vector2Int intCoords = new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
        OnSetSelectedPiece(intCoords);
    }
}
