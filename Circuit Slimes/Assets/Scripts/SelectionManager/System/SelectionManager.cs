﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{

    private IRayProvider       RayProvider;
    private ISelector          Selector;
    private ISelectionResponse SelectionResponse;
    private BoardCoordGetter   BoardCoordGetter;

    public Puzzle.PuzzleController PuzzleController { get; private set; }
    public Transform PuzzleObj { get; private set; }
    public Transform BoardTransform { get; private set; }

    //Outward facing info, fed to the CreatorController
    public Transform CurrentSelection { get; private set; }
    public Vector2Int BoardCoords { get; private set; }
    public bool BoardHover { get; private set; }

    public List<Transform> WhiteList = null;

    #region Initialization

    public void Initialize(Puzzle.PuzzleController puzzleController, Transform puzzleObject)
    {
        //puzzle
        this.PuzzleController = puzzleController;
        this.PuzzleObj = puzzleObject;

        //board
        this.BoardTransform = this.GetPuzzleBoard();

        //ray provider, selector, coordgetter and response
        this.RayProvider       = this.GetComponent<IRayProvider>();
        this.Selector          = this.GetComponent<ISelector>();
        this.SelectionResponse = this.GetComponent<ISelectionResponse>();
        this.BoardCoordGetter  = this.GetComponent<BoardCoordGetter>();
    }

    #endregion


    #region Unity Methods


    private void Update()
    {
        var ray = this.RayProvider.CreateRay();


        //Selection Determination
        var selection = GetSelection(ray);

        //Deselection Response
        if (selection != this.CurrentSelection)
        {
            if(this.CurrentSelection != null)
            {
                this.SelectionResponse.OnDeselect(this.CurrentSelection);
            }

            this.CurrentSelection = selection;

            //Selection Response
            if (this.CurrentSelection != null)
            {
                this.SelectionResponse.OnSelect(this.CurrentSelection);
            }
        }

        //Get Coords from board
        this.BoardCoords = this.GetBoardCoords(ray);    
        this.BoardHover = this.GetBoardHover();

        //Debug.Log(CurrentSelection);
        //Debug.Log(BoardCoords);
        //Debug.Log(BoardHover);
    }

    #endregion


    #region Accessors

    private Transform GetSelection(Ray ray)
    {
        return this.Selector.Check(ray);
    }

    private Vector2Int GetBoardCoords(Ray ray)
    {
        return this.BoardCoordGetter.GetCoords(ray);
    }

    private bool GetBoardHover()
    {
        return this.BoardCoordGetter.GetHover();
    }

    private Transform GetPuzzleBoard()
    {
        return PuzzleObj.Find("Board").transform;
    }

    #endregion
}