using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[RequireComponent(typeof(MaterialSetter))]
[RequireComponent(typeof(IObjectTweener))]
public abstract class Piece : MonoBehaviour
{
    private MaterialSetter materialSetter;
    public Board board { get; private set; }

    public Vector2Int occupiedSquare { get; set; }

    public TeamColor teamColor { get; set; }

    public bool hasMoved { get; private set; }

    public List<Vector2Int> availableMoves;

    private IObjectTweener tweener;

    public abstract List<Vector2Int> SelectAvailableSquares();

    private void Awake()
    {
        availableMoves = new List<Vector2Int>();
        tweener = GetComponent<IObjectTweener>();
        materialSetter = GetComponent<MaterialSetter>();
        hasMoved = false;
    }

    public void SetMaterial(Material mat)
    {
        if(materialSetter == null)
        {
            materialSetter = GetComponent<MaterialSetter>();
        }
        materialSetter.SetSingleMaterial(mat);
    }

    public bool IsAttackingPieceOfType<T>() where T : Piece
    {
        foreach(var square in availableMoves)
        {
            if (board.GetPieceOnSquare(square) is T)
                return true;
        }
        return false;
    }

    public bool IsFromSameTeam(Piece piece)
    {
        return teamColor == piece.teamColor;
    }

    public bool CanMoveTo(Vector2Int coords)
    {
        return availableMoves.Contains(coords);
    }

    public virtual void MovePiece(Vector2Int coords, Vector3 targetPosition)
    {
        occupiedSquare = coords;
        hasMoved = true;
        tweener.MoveTo(transform, targetPosition);
    }

    protected void TryToAddMove(Vector2Int coords)
    {
        availableMoves.Add(coords);
    }

    public void SetData(Vector2Int coords, TeamColor teamColor, Board board, Vector3 targetPosition  )
    {
        this.teamColor = teamColor;
        occupiedSquare = coords;
        this.board = board;
        transform.position = targetPosition;
    }

}
