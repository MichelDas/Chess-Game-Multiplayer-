using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SquareSelector))]
public abstract class Board : MonoBehaviour
{
    [SerializeField] private Transform bootomLeftSquareTransform;
    [SerializeField] private float squareSize;

    private Piece[,] grid;
    [SerializeField] private Piece selectedPiece;
    private ChessGameController chessGameController;
    [SerializeField] private SquareSelector squareSelector;

    public const int BOARD_SIZE = 8;

    public abstract void OnSelectedPieceMoved(Vector2 coords);
    public abstract void SetSelectedPiece(Vector2 coords);

    protected virtual void Awake()
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

    

    // this will be called when player clicks on the board
    public void OnSquareSelected(Vector3 inputPosition)
    {
        if (!chessGameController || !chessGameController.CanPerformMove())
            return;

        Vector2Int coords = CalculateCoordsFromPosition(inputPosition);
        Piece piece = GetPieceOnSquare(coords);

        // When a piece is already selected
        if (selectedPiece)
        {
            // if I am selecting the piece that is already selecting again
            if (piece != null && selectedPiece == piece)
                DeselectPiece();
            // if I am selecting a different piece of my team
            else if (piece != null && selectedPiece != piece && chessGameController.IsTeamTurnActive(piece.teamColor))
                SelectPiece(coords);
            // if I am selecting a square where the piece can be moved
            else if (selectedPiece.CanMoveTo(coords))
                OnSelectedPieceMoved((Vector2)coords);
        }
        // when No piece is selected
        else
        {
            // if the piece clicked, is of my color
            // select it
            if (piece != null && chessGameController.IsTeamTurnActive(piece.teamColor))
            {
                SelectPiece(coords);
            }
        }
    }

    private void SelectPiece(Vector2Int coords)
    {
        Piece piece = GetPieceOnSquare(coords);
        chessGameController.RemoveMovesEnablingAttackOnPieceOfType<King>(piece); // ei line ta bujhi nai

        //SelectPiece()  -> SetSelectPiece() -> this.OnSetSelectedPiece()
        SetSelectedPiece(coords);
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

    // this is called when a piece is moved
    // in a coords  
    public void MoveSelectedPiece(Vector2Int coords)
    {
        // opponent er piece kete fela
        TryToTakeOppositePiece(coords);

        UpdateBoardOnPieceMove(coords, selectedPiece.occupiedSquare, selectedPiece, null);
        selectedPiece.MovePiece(coords, CalculatePositionFromCoords(coords));
        DeselectPiece();
        EndTurn();
    }

    // this will eleminate the piece on coords and
    // put piece there
    private void TryToTakeOppositePiece(Vector2Int coords)
    {
        Piece piece = GetPieceOnSquare(coords);

        if (piece != null && !selectedPiece.IsFromSameTeam(piece))
            TakePiece(piece);
    }

    private void TakePiece(Piece piece)
    {
        if (piece)
        {
            grid[piece.occupiedSquare.x, piece.occupiedSquare.y] = null;
            chessGameController.OnPieceRemoved(piece);
        }
    }

    public Piece GetPieceOnSquare(Vector2Int coords)
    {
        if (CheckIfCoordinatesAreOnBoard(coords))
        {
            return grid[coords.x, coords.y];
        }
        return null;
    }

    // eita newPiece k newCoords a, ar oldPiece k oldCoords a boshai dibe
    // piece move korar belai, je square a nite chacchi oi square er oldPiece null hobe,
    // oldCoords hobe new piece er current position, newCoords hobe jekhane move korte chai.
    public void UpdateBoardOnPieceMove(Vector2Int NewCoords, Vector2Int oldCoords, Piece newPiece, Piece oldPiece)
    {
        grid[oldCoords.x, oldCoords.y] = oldPiece;
        grid[NewCoords.x, NewCoords.y] = newPiece;
    }

    // this will get the piece in the coords and
    // assign it in the selectedPiece
    public void OnSetSelectedPiece(Vector2Int intCoords)
    {
        Piece piece = GetPieceOnSquare(intCoords);
        selectedPiece = piece;
    }

    private void EndTurn()
    {
        chessGameController.EndTurn();
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

    public Vector3 CalculatePositionFromCoords(Vector2Int coords)
    {
        return bootomLeftSquareTransform.position + new Vector3(coords.x * squareSize, 0f, coords.y * squareSize);
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

    internal void OnGameRestarted()
    {
        selectedPiece = null;
        CreateGrid();
    }



}
