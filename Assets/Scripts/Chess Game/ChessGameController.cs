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

    public void InitializeGame(UIManager uIManager, Board board, CameraSetup cameraSetup)
    {
        this.uIManager = uIManager;
        this.board = board;
        this.cameraSetup = cameraSetup;
        CreatePlayers();
    }

    private void CreatePlayers()
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

        if (team == TeamColor.Black)
            newPiece.transform.eulerAngles = newPiece.transform.eulerAngles + 180.0f * Vector3.up;
        newPiece.transform.SetParent(board.transform);
        newPiece.SetData(squareCoords, team, board, board.CalculatePositionFromCoords(squareCoords));

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
        GenerateAllPossiblePlayerMoves(GetopponentToCurrentPlayer(activePlayer));
        if (CheckIfGameIsFinished())
            EndGame();
        else
            ChangeActiveTeam();
    }

    private bool CheckIfGameIsFinished()
    {

        //if (kingAttackingPieces.Length > 0)
        //{
        //    ChessPlayer oppositePlayer = GetOpponentToPlayer(activePlayer);
        //    Piece attackedKing = oppositePlayer.GetPiecesOfType<King>().FirstOrDefault();
        //    oppositePlayer.RemoveMovesEnablingAttakOnPieceOfType<King>(activePlayer, attackedKing);

        //    int avaliableKingMoves = attackedKing.avaliableMoves.Count;
        //    if (avaliableKingMoves == 0)
        //    {
        //        bool canCoverKing = oppositePlayer.CanHidePieceFromAttack<King>(activePlayer);
        //        if (!canCoverKing)
        //            return true;
        //    }
        //}
        //return false;

        Piece[] kingAttackingPieces = activePlayer.GetPiecesAttackingOppositePieceOfType<King>();
        Debug.Log("checking if game is finished");

        // if the king is checked
        if (kingAttackingPieces.Length > 0)
        {
            Debug.Log("Some piece is attacking king");
            // getting opponent reference
            ChessPlayer oppositePlayer = GetopponentToCurrentPlayer(activePlayer);

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

    public void RestartGame()
    {
        DestroyPieces();
        board.OnGameRestarted();
        whitePlayer.OnGameRestarted();
        blackPlayer.OnGameRestarted();
        StartNewGame();
    }

    private void DestroyPieces()
    {
        whitePlayer.activePieces.ForEach(p => Destroy(p.gameObject));
        blackPlayer.activePieces.ForEach(p => Destroy(p.gameObject));
    }

    public void OnPieceRemoved(Piece piece)
    {
        ChessPlayer pieceOwner = piece.teamColor == TeamColor.White ? whitePlayer : blackPlayer;
        pieceOwner.RemovePiece(piece);
        Destroy(piece.gameObject);
    }

    private void ChangeActiveTeam()
    {
        activePlayer = activePlayer == whitePlayer ? blackPlayer : whitePlayer;
    }

    private ChessPlayer GetopponentToCurrentPlayer(ChessPlayer activePlayer)
    {
        return activePlayer == whitePlayer ? blackPlayer : whitePlayer;
    }

    internal bool IsTeamTurnActive(TeamColor team)
    {
        return activePlayer.teamColor == team;
    }

    public bool IsGameInProgress()
    {
        return state == GameState.Play;
    }

    // This will remove all moves where the king will be checked
    public void RemoveMovesEnablingAttackOnPieceOfType<T>(Piece piece) where T : Piece
    {
        activePlayer.RemoveMovesEnablingAttackOnPiece<T>(GetopponentToCurrentPlayer(activePlayer), piece);
    }



}
