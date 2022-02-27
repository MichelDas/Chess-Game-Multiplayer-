using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerBoard : Board
{
    public override void OnSelectedPieceMoved(Vector2 coords)
    {
        // vector2 coords k vector2Int coords a convert korbo
        Vector2Int intCoords = new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
        // then Board Class a implement kora MoveSelectedPiece method k call korlam 
        MoveSelectedPiece(intCoords);
    }

    public override void SetSelectedPiece(Vector2 coords)
    {
        // vector2 coords k vector2Int coords a convert korbo
        Vector2Int intCoords = new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
        // then Board Class a implement kora OnSetSelectedPiece method k call korlam 
        OnSetSelectedPiece(intCoords);
    }

    
}
