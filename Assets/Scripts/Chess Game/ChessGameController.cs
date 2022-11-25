
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(PiecesCreator))]
public abstract class ChessGameController : MonoBehaviour
{
    protected const byte SET_GAME_STATE_EVENT_CODE = 1;

    [SerializeField] private BoardLayout startingBoardLayout;

    private ChessUIManager UIManager;
    public CameraSetup cameraSetup;
    private Board board;
    private PiecesCreator pieceCreator;
    protected ChessPlayer whitePlayer;
    protected ChessPlayer blackPlayer;
    protected ChessPlayer activePlayer;

    protected GameState state;

    private void Awake()
    {
        pieceCreator = GetComponent<PiecesCreator>();
    }

    #region Abstract methods
    protected abstract void SetGameState(GameState state);
    public abstract void TryToStartThisGame();
    public abstract bool CanPerformMove();
    public abstract bool IsTeamLocal(TeamColor color);
    public abstract bool IsTeamTurnActive(TeamColor team);
    public abstract bool CheckIfGameIsFinished();
    public abstract void LeaveAfterEnd();
    public abstract void EndGame(ChessUIManager UIManager);
    public abstract void BackToTheMainMenu();
    #endregion

    internal void SetDependencies(CameraSetup cameraSetup, ChessUIManager UIManager, Board board)
    {
        this.cameraSetup = cameraSetup;
        this.UIManager = UIManager;
        this.board = board;
    }

    public void InitializeGame()
    {     
        CreatePlayers();       
    }

    private void CreatePlayers()
    {
        whitePlayer = new ChessPlayer(TeamColor.White, board);
        blackPlayer = new ChessPlayer(TeamColor.Black, board);
    }

    // СТАРТ ИГРЫ
    public void StartNewGame()
    {
        // установка начального ui
        UIManager.OnGameStarted();
        // установка некоего состояния игры - инициализация
        SetGameState(GameState.Init);       
        // создаём поле по частям из заданного объекта с реализацией разлиновки
        CreatePiecesFromLayout(startingBoardLayout);
        // активный игрок - белый
        activePlayer = whitePlayer;
        // просчитываем все доступные активному игроку ходы
        GenerateAllPossiblePlayerMoves(activePlayer);
        // что же, самое попытаться начать эту игру
        Debug.Log("Begin Trying Start Game");
        TryToStartThisGame();
    }

    internal bool IsGameInProgress()
    {
        return state == GameState.Play;
    }
    public bool IsGameFinished()
    {
        return state == GameState.Finished;
    }

    private void CreatePiecesFromLayout(BoardLayout layout)
    {
        for (int i = 0; i < layout.GetPiecesCount(); i++)
        {
            // получаем координаты квадратика по индексу
            Vector2Int squareCoords = layout.GetSquareCoordsAtIndex(i);
            // так же с цветом
            TeamColor team = layout.GetSquareTeamColorAtIndex(i);
            // и название пешки на нём
            string typeName = layout.GetSquarePieceNameAtIndex(i);

            // когда получили название пешки по индексу для нынешней по for клетки, то вынимаем класс её типа - кто она там...
            Type type = Type.GetType(typeName);
            // ...и спавним её по всем атрибутам
            CreatePieceAndInitialize(squareCoords, team, type);
        }
    }

    public void CreatePieceAndInitialize(Vector2Int squareCoords, TeamColor team, Type type)
    {
        
        Piece newPiece = pieceCreator.CreatePiece(type).GetComponent<Piece>();

        newPiece.SetData(squareCoords, team, board);
        Material teamMaterial = pieceCreator.DoTeamColored(team, type);
        newPiece.SetMaterial(teamMaterial);
        board.SetPieceOnBoard(squareCoords, newPiece);
        newPiece.SetRotateOnBoard(team);

        ChessPlayer currentPlayer = team == TeamColor.White ? whitePlayer : blackPlayer;
        currentPlayer.AddPiece(newPiece);
    }

	internal void SetupCamera(TeamColor team)
	{
        cameraSetup.SetupCamera(team);
    }

    private void GenerateAllPossiblePlayerMoves(ChessPlayer player)
    {
        player.GenerateAllPossibleMoves();
    }

    // Завершение хода
    public void EndTurn()
    {
        GenerateAllPossiblePlayerMoves(activePlayer);
        GenerateAllPossiblePlayerMoves(GetOpponentToPlayer(activePlayer));

        if (CheckIfGameIsFinished())
        {
            EndGame(UIManager);
            Debug.Log("This game over");
        }
        else
        {
            ChangeActiveTeam();
        }
    }

    // Перезапуск игры РАБОТАЕТ КАК ПЕРЕМЕЩЕНИЕ В ГЛАВНОЕ МЕНЮ ПО КНОПКЕ!
    public void RestartGame()
    {
        if (whitePlayer != null && blackPlayer != null)
        {
            DestroyPieces();
            board.OnGameRestarted();
            whitePlayer.OnGameRestarted();
            blackPlayer.OnGameRestarted();
        }
        BackToTheMainMenu();
    }

    private void DestroyPieces()
    {
        whitePlayer.activePieces.ForEach(p => Destroy(p.gameObject));
        blackPlayer.activePieces.ForEach(p => Destroy(p.gameObject));
    }

    private void ChangeActiveTeam()
    {
        activePlayer = activePlayer == whitePlayer ? blackPlayer : whitePlayer;
    }

    public ChessPlayer GetOpponentToPlayer(ChessPlayer player)
    {
        return player == whitePlayer ? blackPlayer : whitePlayer;
    }

    internal void OnPieceRemoved(Piece piece)
    {
        ChessPlayer pieceOwner = (piece.team == TeamColor.White) ? whitePlayer : blackPlayer;
        Debug.Log("Call Remove from player in owner");
        pieceOwner.RemovePiece(piece);
    }

    internal void RemoveMovesEnablingAttakOnPieceOfType<T>(Piece piece) where T : Piece
    {
        if(piece.team == TeamColor.Black)
            blackPlayer.RemoveMovesEnablingAttakOnPieceOfType<T>(GetOpponentToPlayer(blackPlayer), piece);
        else
            whitePlayer.RemoveMovesEnablingAttakOnPieceOfType<T>(GetOpponentToPlayer(whitePlayer), piece);
    }
}

