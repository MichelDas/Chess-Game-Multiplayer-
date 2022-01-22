using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SquareSelector))]
public class Board : MonoBehaviour
{
    [SerializeField] private Transform bootomLeftSquareTransform;
    [SerializeField] private float squareSize;

    [SerializeField] private Piece[,] grid;
    [SerializeField] private Piece selectedPiece;
    private ChessGameController chessGameController;
    private SquareSelector squareSelector;

    public const int BOARD_SIZE = 8;

    private void Awake()
    {
        squareSelector = GetComponent<SquareSelector>();
        CreateGrid();
    }

    public void SetDependencies(ChessGameController chessGameController)
    {
        this.chessGameController = chessGameController;
    }

    private void CreateGrid()
    {
        grid = new Piece[BOARD_SIZE, BOARD_SIZE];
    }

    public  Vector3 CalculatePositionFromCoords(Vector2Int coords)
    {
        return bootomLeftSquareTransform.position + new Vector3(coords.x * squareSize, 0f, coords.y * squareSize);
    }

    // this will be called when player clicks on the board
    public void OnSquareSelected(Vector3 inputPosition)
    {
        Vector2Int coords = CalculateCoordsFromPosition(inputPosition);
        Piece piece = GetPieceOnSquare(coords);

        // When a piece is already selected
        if (selectedPiece)
        {
            if (piece != null && selectedPiece == piece)
                DeselectPiece();
            else if (piece != null && selectedPiece != piece && chessGameController.IsTeamTurnActive(piece.team))
                SelectPiece(piece);
            else if (selectedPiece.CanMoveTo(coords))
                OnSelectedPieceMoved(coords, selectedPiece);
        }
        // when No piece is selected
        else
        {
            // if the piece clicked on is out of my color
            // select it
            if (piece != null && chessGameController.IsTeamTurnActive(piece.team))
            {
                SelectPiece(piece);
            }
        }
    }


    private void SelectPiece(Piece piece)
    {
        selectedPiece = piece;
        List<Vector2Int> selection = selectedPiece.availableMoves;
        ShowSelectionSquares(selection);
    }

    private void ShowSelectionSquares(List<Vector2Int> selection)
    {
        Dictionary<Vector3, bool> squaresData = new Dictionary<Vector3, bool>();
        for(int i=0; i<selection.Count; i++)
        {
            Vector3 position = CalculatePositionFromCoords(selection[i]);
            bool isSquareFree = GetPieceOnSquare(selection[i]) == null;
            squaresData.Add(position, isSquareFree);
        }

        squareSelector.ShowSelection(squaresData);
    }

    private void DeselectPiece()
    {
        selectedPiece = null;
        squareSelector.ClearSelection();
    }

    private void OnSelectedPieceMoved(Vector2Int coords, Piece piece)
    {
        UpdateBoardOnPieceMove(coords, piece.occupiedSquare, piece, null);
        selectedPiece.MovePiece(coords);
        DeselectPiece();
        EndTurn();
    }

    private void EndTurn()
    {
        chessGameController.EndTurn();
    }

    private void UpdateBoardOnPieceMove(Vector2Int NewCoords, Vector2Int oldCoords, Piece newPiece, Piece oldPiece)
    {
        grid[oldCoords.x, oldCoords.y] = oldPiece;
        grid[NewCoords.x, NewCoords.y] = newPiece;
    }

    public Piece GetPieceOnSquare(Vector2Int coords)
    {
        if (CheckIfCoordinatesAreOnBoard(coords))
        {
            return grid[coords.x, coords.y];
        }
        return null;

    }

    public bool CheckIfCoordinatesAreOnBoard(Vector2Int coords)
    {
        if (coords.x < 0 || coords.y < 0 || coords.x >= BOARD_SIZE || coords.y >= BOARD_SIZE)
            return false;
        return true;
    }

    private Vector2Int CalculateCoordsFromPosition(Vector3 inputPosition)
    {
        int x = Mathf.FloorToInt(inputPosition.x / squareSize) + BOARD_SIZE / 2;
        int y = Mathf.FloorToInt(inputPosition.z / squareSize) + BOARD_SIZE / 2;

        return new Vector2Int(x, y);

    }

    public bool HasPiece(Piece piece)
    {
        for(int i=0; i<BOARD_SIZE; i++)
        {
            for(int j=0; j<BOARD_SIZE; j++)
            {
                if(grid[i,j] == piece)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void SetPieceOnBoard(Vector2Int squareCoords, Piece newPiece)
    {
        if (CheckIfCoordinatesAreOnBoard(squareCoords))
            grid[squareCoords.x, squareCoords.y] = newPiece;
    }

}