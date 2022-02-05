using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[RequireComponent(typeof(PieceCreator))]
public abstract class ChessGameController : MonoBehaviour
{
    public enum GameState { Init, Play, Finished }

    [SerializeField] private BoardLayout startingBoardLayout;
    private Board board;
    private UIManager uIManager;
    private CameraSetup cameraSetup;

    private PieceCreator pieceCreator;
    protected ChessPlayer whitePlayer;
    protected ChessPlayer blackPlayer;
    protected ChessPlayer activePlayer;

    protected GameState state;

    protected abstract void SetGameState(GameState state);
    public abstract void TryToStartCurrentGame();
    public abstract bool CanPerformMove();

    private void Awake()
    {
        pieceCreator = GetComponent<PieceCreator>();
    }

    void Start()
    {
        StartNewGame();     
    }

    public void SetDependencies(UIManager uIManager, Board board, CameraSetup cameraSetup)
    {
        this.uIManager = uIManager;
        this.board = board;
        this.cameraSetup = cameraSetup;
    }

    public void StartNewGame()
    {
        uIManager.OnGameStarted();
        SetGameState(GameState.Init);
        CreatePiecesFromLayout(startingBoardLayout);
        activePlayer = whitePlayer;
        GenerateAllPossiblePlayerMoves(activePlayer);
        TryToStartCurrentGame();
    }

    public void SetupCAmera(TeamColor team)
    {
        cameraSetup.SetupCamera(team);
    }

    public void CreatePlayers()
    {
        whitePlayer = new ChessPlayer(TeamColor.White, board);
        blackPlayer = new ChessPlayer(TeamColor.Black, board);
    }

    private void CreatePiecesFromLayout(BoardLayout layout)
    {
        for(int i=0; i<layout.GetPiecesCount(); i++)
        {
            Vector2Int squareCoords = layout.GetSquareCoordsAtIndex(i);
            TeamColor team = layout.GetSquareTeamColorAtIndex(i);
            string typeName = layout.GetSquarePieceNameAtIndex(i);

            Type type = Type.GetType(typeName);
            CreatePieceAndInitialize(squareCoords, team, type);
        }
    }

    private void CreatePieceAndInitialize(Vector2Int squareCoords, TeamColor team, Type type)
    {
        Piece newPiece = pieceCreator.CreatePiece(type).GetComponent<Piece>();
        newPiece.SetData(squareCoords, team, board);

        Material teamMaterial = pieceCreator.GetTeamMaterial(team);
        newPiece.SetMaterial(teamMaterial);

        board.SetPieceOnBoard(squareCoords, newPiece);

        ChessPlayer currentPlayer = team == TeamColor.White ? whitePlayer : blackPlayer;
        currentPlayer.AddPiece(newPiece);
    }

    private void GenerateAllPossiblePlayerMoves(ChessPlayer player)
    {
        player.GenerateAllPossibleMoves();
    }

    public void EndTurn()
    {
        GenerateAllPossiblePlayerMoves(activePlayer);
        GenerateAllPossiblePlayerMoves(GetopponentToPlayer(activePlayer));
        if (CheckIfGameIsFinished())
            EndGame();
        else
            ChangeActiveTeam();
    }

    private bool CheckIfGameIsFinished()
    {
        Piece[] kingAttackingPieces = activePlayer.GetPiecesAttackingOppositePieceOfType<King>();

        // if the king is checked
        if (kingAttackingPieces.Length > 0)
        {
            // getting opponent reference
            ChessPlayer oppositePlayer = GetopponentToPlayer(activePlayer);

            // getting the reference of the king
            Piece attackedKing = oppositePlayer.GetPiecesOfType<King>().FirstOrDefault();

            // remove the movable places of the oponent's King
            // where the king will be checked
            oppositePlayer.RemoveMovesEnablingAttackOnPiece<King>(activePlayer, attackedKing);

            int availableKingMoves = attackedKing.availableMoves.Count;

            if(availableKingMoves == 0)
            {
                bool canCoverKing = oppositePlayer.CanHidePieceFromAttack<King>(activePlayer);

                if (!canCoverKing)
                    return true;
            }
        }
        return false;

    }

    private void EndGame()
    {
        Debug.Log("Game Ended");
        uIManager.OnGameFinished(activePlayer.teamColor.ToString());
        SetGameState(GameState.Finished);
    }

    public void OnPieceRemoved(Piece piece)
    {
        ChessPlayer pieceOwner = piece.team == TeamColor.White ? whitePlayer : blackPlayer;
        pieceOwner.RemovePiece(piece);
        Destroy(piece.gameObject);
    }

    private void ChangeActiveTeam()
    {
        activePlayer = activePlayer == whitePlayer ? blackPlayer : whitePlayer;
    }

    private ChessPlayer GetopponentToPlayer(ChessPlayer activePlayer)
    {
        return activePlayer == whitePlayer ? blackPlayer : whitePlayer;
    }

    internal bool IsTeamTurnActive(TeamColor team)
    {
        return activePlayer.teamColor == team;
    }

    //private void SetGameState(GameState state)
    //{
    //    this.state = state;
    //}

    public bool IsGameInProgress()
    {
        return state == GameState.Play;
    }

    // This will remove all moves where the king will be checked
    public void RemoveMovesEnablingAttackOnPieceOfType<T>(Piece piece) where T : Piece
    {
        activePlayer.RemoveMovesEnablingAttackOnPiece<T>(GetopponentToPlayer(activePlayer), piece);
    }



}
