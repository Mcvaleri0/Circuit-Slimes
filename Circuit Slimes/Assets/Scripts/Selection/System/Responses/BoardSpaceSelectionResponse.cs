using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpaceSelectionResponse : MonoBehaviour, IBoardSelectionResponse
{
    //visual selection of the board space
    public GameObject BoardSpaceSelection;
    private Transform BoardSpaceSelectionTransform;
    private MeshRenderer BoardSpaceSelectionRenderer;

    private SelectionManager Manager;

    public void SelectionInit()
    {
        var boardSpaceSelection = Instantiate(BoardSpaceSelection, this.Manager.PuzzleTransform);

        this.BoardSpaceSelectionTransform = boardSpaceSelection.transform;
        this.BoardSpaceSelectionRenderer = boardSpaceSelection.GetComponent<MeshRenderer>();

        this.BoardSpaceSelectionRenderer.enabled = false;
    }

    public void Start()
    {
        this.Manager = this.GetComponent<SelectionManager>();

        this.SelectionInit();
    }

    public void Update(Vector2Int BoardCoords, bool BoardHover) {

        if(BoardSpaceSelectionRenderer == null || BoardSpaceSelectionTransform == null)
        {
            SelectionInit();
        }

        this.BoardSpaceSelectionRenderer.enabled = BoardHover;
        var spacepos = Puzzle.Board.LevelBoard.WorldCoords(BoardCoords);
        spacepos.y += 0.1f;
        this.BoardSpaceSelectionTransform.position = spacepos;
    }
}
