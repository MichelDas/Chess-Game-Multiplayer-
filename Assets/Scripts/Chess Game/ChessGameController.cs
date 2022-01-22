using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[RequireComponent(typeof(PieceCreator))]
public class ChessGameController : MonoBehaviour
{
    public enum GameState { Init, Play, Finished }

    [SerializeField] private BoardLayout startingBoardLayout;
    [SerializeField] private Board board;

    private PieceCreator pieceCreator;
    private ChessPlayer whitePlayer;
    private ChessPlayer blackPlayer;
    private ChessPlayer activePlayer;

    public GameState state;

    private void Awake()
    {
        SetDependencies();
        CreatePlayers();
    }

    private void CreatePlayers()
    {
        whitePlayer = new ChessPlayer(TeamColor.White, board);
        blackPlayer = new ChessPlayer(TeamColor.Black, board);
    }

    private void SetDependencies()
    {
        pieceCreator = GetComponent<PieceCreator>();
    }

    void Start()
    {
        StartNewGame();
        
    }

    private void StartNewGame()
    {
        SetGameState(GameState.Init);
        board.SetDependencies(this);
        CreatePiecesFromLayout(startingBoardLayout);
        activePlayer = whitePlayer;
        GenerateAllPossiblePlayerMoves(activePlayer);
        SetGameState(GameState.Play);
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
        SetGameState(GameState.Finished);
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

    private void SetGameState(GameState state)
    {
        this.state = state;
    }

    public bool IsGameInProgress()
    {
        return state == GameState.Play;
    }

}
