using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    Vector2Int[] offsets = new Vector2Int[]
    {
        new Vector2Int(2,1),
        new Vector2Int(2,-1),
        new Vector2Int(1,2),
        new Vector2Int(1,-2),
        new Vector2Int(-2,1),
        new Vector2Int(-2,-1),
        new Vector2Int(-1,2),
        new Vector2Int(-1,-2),
    };

    public override List<Vector2Int> SelectAvailableSquares()
    {
        availableMoves.Clear();
        for(int i=0; i<offsets.Length; i++)
        {
            Vector2Int nextCoords = occupiedSquare + offsets[i];
            Piece piece = board.GetPieceOnSquare(nextCoords);

            if (!board.CheckIfCoordinatesAreOnBoard(nextCoords))
                continue;

            // jodi oi space a kono piece na thake or
            // jei piece ta ase oita amr team er na hoi
            // ami oi ghor a jete parbo
            if (piece == null || !piece.IsFromSameTeam(this))
                TryToAddMove(nextCoords);
        }
        return availableMoves;
    }
}
