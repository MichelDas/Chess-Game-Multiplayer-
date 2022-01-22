using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class ChessPlayer
{

    public TeamColor teamColor;

    public Board board { get; set; }

    public List<Piece> activePieces { get; private set; }

    public ChessPlayer(TeamColor teamColor, Board board)
    {
        this.board = board;
        this.teamColor = teamColor;
        activePieces = new List<Piece>();
    }

    public void AddPiece(Piece piece)
    {
        if (!activePieces.Contains(piece))
            activePieces.Add(piece);
    }

    public void RemovePiece(Piece piece)
    {
        if (activePieces.Contains(piece))
            activePieces.Remove(piece);
    }

    public void GenerateAllPossibleMoves()
    {
        foreach(var piece in activePieces)
        {
            if (board.HasPiece(piece))
                piece.SelectAvailableSquares();
        }
    }

    // Here we will find all pieces that attack
    // oponent's T typed piece
    // for example this will list all pieces that attack the king
    public Piece[] GetPiecesAttackingOppositePieceOfType<T>() where T : Piece
    {
        return activePieces.Where(p => p.IsAttackingPieceOfType<T>()).ToArray();
    }

    // for example: get the Pawns or Kinghts
    public Piece[] GetPiecesOfType<T>() where T : Piece
    {
        return activePieces.Where(p => p is T).ToArray();
    }

    public void RemoveMovesEnablingAttackOnPiece<T>(ChessPlayer oponent, Piece selectedPiece) where T : Piece
    {
        // example: selectedpiece is King
        // oponent is the player who is checking the king that is me

        List<Vector2Int> coordsToRomove = new List<Vector2Int>();

        foreach(var coords in selectedPiece.availableMoves)
        {
            Piece pieceOnSquare = board.GetPieceOnSquare(coords);
            board.UpdateBoardOnPieceMove(coords, selectedPiece.occupiedSquare, selectedPiece, null);

            if(oponent.CheckIfIsAttackingPiece<T>())
                coordsToRomove.Add(coords);
        }

        foreach(var coords in coordsToRomove)
        {
            selectedPiece.availableMoves.Remove(coords);
        }
    }

    private bool CheckIfIsAttackingPiece<T>() where T : Piece
    {
        foreach(var piece in activePieces)
        {
            if (board.HasPiece(piece) && piece.IsAttackingPieceOfType<T>())
                return true;
        }
        return false;
    }

    public bool CanHidePieceFromAttack<T>(ChessPlayer opponent) where T : Piece
    {
        foreach(var piece in activePieces)
        {
            foreach(var coords in piece.availableMoves)
            {
                Piece pieceOnCoords = board.GetPieceOnSquare(coords);
                board.UpdateBoardOnPieceMove(coords, piece.occupiedSquare, piece, null);
                opponent.GenerateAllPossibleMoves();
                if (!opponent.CheckIfIsAttackingPiece<T>())
                {
                    board.UpdateBoardOnPieceMove(piece.occupiedSquare, coords, piece, pieceOnCoords);
                    return true;
                }
                board.UpdateBoardOnPieceMove(piece.occupiedSquare, coords, piece, pieceOnCoords);


            }
        }
        return false;
    }
}
