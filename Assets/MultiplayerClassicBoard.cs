using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class MultiplayerClassicBoard : Board, IOnEventCallback
{
    private PhotonView photonView;

    protected override void Awake()
    {
        base.Awake();
        photonView = GetComponent<PhotonView>();
    }

    #region RPC Events
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void SelectedPieceMoved(Vector2 coords, Piece piece)
    {
        Vector2 piece_coords = new Vector2(piece.occupiedSquare.x, piece.occupiedSquare.y);
        object[] content = new object[] { coords, piece_coords };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(MOVE_PIECE_ON_POSITION, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == MOVE_PIECE_ON_POSITION)
        {
            object[] data = (object[])photonEvent.CustomData;
            Vector2 coords = (Vector2)data[0];
            Vector2 piece_coords = (Vector2)data[1];

            Debug.LogError("Piece is move!");

            Vector2Int intCoords = new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
            Vector2Int pieceCoords = new Vector2Int((int)piece_coords.x, (int)piece_coords.y);
            Debug.Log($"Event in <MultiBoard>. Use <On seleted piece moved>.");
            OnSelectedPieceMoved(intCoords, pieceCoords);
        }
    }
    #endregion

    public override void SetSelectedPiece(Vector2 coords)
    {
        Debug.Log("Set selected piece");
        Vector2Int intCoords = new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
        OnSetSelectedPiece(intCoords);
    }
}