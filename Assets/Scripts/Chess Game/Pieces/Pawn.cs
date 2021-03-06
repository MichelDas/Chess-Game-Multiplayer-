using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public override List<Vector2Int> SelectAvailableSquares()
    {
        availableMoves.Clear();
        Vector2Int direction = teamColor == TeamColor.White ? Vector2Int.up : Vector2Int.down;
        float range = hasMoved ? 1 : 2;

        for(int i=1; i<=range; i++)
        {
            Vector2Int nextCoords = occupiedSquare + direction * i;
            Piece piece = board.GetPieceOnSquare(nextCoords);

            if (!board.CheckIfCoordinatesAreOnBoard(nextCoords))
                break;
            if (piece == null)
                TryToAddMove(nextCoords);
            else if (piece.IsFromSameTeam(this))
                break;
        }

        // diagonal direction for enemy
        Vector2Int[] takeDirections = new Vector2Int[]
        {
            new Vector2Int(1, direction.y),
            new Vector2Int(-1, direction.y)
        };

        for(int i=0; i< takeDirections.Length; i++)
        {
            Vector2Int nextCoords = occupiedSquare + takeDirections[i];
            Piece piece = board.GetPieceOnSquare(nextCoords);
            if (!board.CheckIfCoordinatesAreOnBoard(nextCoords))
                break;

            if (piece != null && !piece.IsFromSameTeam(this))
                TryToAddMove(nextCoords);
        }

        return availableMoves;

    }
}
