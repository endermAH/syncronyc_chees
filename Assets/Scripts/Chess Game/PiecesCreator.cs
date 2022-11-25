using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PiecesCreator : MonoBehaviour
{
    [SerializeField] private GameObject[] piecesPrefabs;

    [Header("Materials for Pieces")]
    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material whiteMaterial;

    [SerializeField] private Material KnightBlackMaterial;
    [SerializeField] private Material KnightWhiteMaterial;

    [SerializeField] private Material BishopBlackMaterial;
    [SerializeField] private Material BishopWhiteMaterial;

    [SerializeField] private Material KingBlackMaterial;
    [SerializeField] private Material KingWhiteMaterial;

    [SerializeField] private Material QueenBlackMaterial;
    [SerializeField] private Material QueenWhiteMaterial;

    [SerializeField] private Material RookBlackMaterial;
    [SerializeField] private Material RookWhiteMaterial;

    private Dictionary<string, GameObject> nameToPieceDict = new Dictionary<string, GameObject>();

    private void Awake()
    {
        foreach (var piece in piecesPrefabs)
        {
            nameToPieceDict.Add(piece.GetComponent<Piece>().GetType().ToString(), piece);
        }
    }

    public GameObject CreatePiece(Type type)
    {
        GameObject prefab = nameToPieceDict[type.ToString()];
        if (prefab)
        {
            GameObject newPiece = Instantiate(prefab);
            return newPiece;
        }
        return null;
    }

    public Material GetTeamMaterial(TeamColor team)
    {
        return team == TeamColor.White ? whiteMaterial : blackMaterial;
    }

    public Material DoTeamColored(TeamColor team, Type type)
    {  
        switch (type.ToString())
        {
            case "Knight":
                if (team == TeamColor.White)
                    return KnightWhiteMaterial;
                else
                    return KnightBlackMaterial;
            case "Bishop":
                if (team == TeamColor.White)
                    return BishopWhiteMaterial;
                else
                    return BishopBlackMaterial;
            case "King":
                if (team == TeamColor.White)
                    return KingWhiteMaterial;
                else
                    return KingBlackMaterial;
            case "Queen":
                if (team == TeamColor.White)
                    return QueenWhiteMaterial;
                else
                    return QueenBlackMaterial;
            case "Rook":
                if (team == TeamColor.White)
                    return RookWhiteMaterial;
                else
                    return RookBlackMaterial;
            default:
                return team == TeamColor.White ? whiteMaterial : blackMaterial;

        }
    }
}
