﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Puzzle.Actions;
using Puzzle.Board;

namespace Puzzle.Pieces
{
    public class Agent : Piece
    {
        protected int StartTurn;

        public int Turn;

        public enum States
        {
            Idle,
            Think,
            Act,
            Rewind,
            Restart,
            Waiting,
            Inactive
        };

        public States State;

        protected List<Action> KnownActions;

        protected Action ChosenAction;

        public struct Statistics
        {
            public int MaxHP, HP, Speed, MaxFood, Food;

            public Statistics(int maxhp, int speed, int maxfood)
            {
                this.MaxHP = maxhp;
                this.HP = maxhp;

                this.Speed = speed;

                this.MaxFood = maxfood;
                this.Food = maxfood;
            }
        }

        public Statistics Stats { get; protected set; }

        public LevelBoard.Directions Orientation { get; set; }

        protected struct AgentState
        {
            public Vector2Int Coords { get; private set; }

            public Statistics Stats { get; private set; }

            public LevelBoard.Directions Orientation { get; private set; }

            public Action ChosenAction { get; private set; }

            public AgentState(Vector2Int coords, Statistics stats, LevelBoard.Directions ori, Action chosen)
            {
                this.Coords = coords;

                this.Stats = stats;

                this.Orientation = ori;

                this.ChosenAction = chosen;
            }
        }

        protected List<AgentState> StateLog;


        // Init Method
        public virtual void Initialize(LevelBoard board, Vector2Int coords, Categories category,
            LevelBoard.Directions ori = 0, int turn = 0)
        {
            base.Initialize(board, coords, category);

            this.State = States.Idle;

            this.KnownActions = new List<Action>();

            this.StateLog = new List<AgentState>();

            this.Orientation = ori;

            this.StartTurn = turn;
            this.Turn = turn;
        }


        #region === Unity Methods ===
        // Use this for initialization
        new protected virtual void Start()
        {
            this.State = States.Idle;
        }

        // Update is called once per frame
        new protected virtual void Update()
        {
            switch (this.State)
            {
                default:
                case States.Idle:
                case States.Waiting:
                case States.Inactive:
                    break;

                case States.Think:
                    // If there is a log of the current turn
                    if (this.StateLogExists(this.Turn))
                    {
                        this.ChosenAction = this.StateLog[this.Turn].ChosenAction;
                    }

                    // If there is none
                    else
                    {
                        // Decide on an Action
                        this.ChosenAction = Think();

                        if (this.ChosenAction == null) return;

                        // Save a log of this State
                        this.SaveState();
                    }

                    // Go to Act
                    this.State = States.Act;
                    break;

                case States.Act:
                    // If the Action has been completed
                    if (this.ChosenAction.Execute(this))
                    {
                        // Go to waiting
                        this.State = States.Waiting;

                        this.ChosenAction = null;

                        this.Turn++;
                    }
                    break;

                case States.Rewind:
                    this.Turn--;

                    this.LoadState();

                    this.State = States.Waiting;
                    break;

                case States.Restart:
                    this.Turn = this.StartTurn;

                    this.LoadState();

                    this.State = States.Idle;
                    break;
            }
        }
        #endregion


        #region === Agent Methods ===
        // Returns the Chosen Action on a given Turn
        public virtual Action Think()
        {
            return null;
        }
        #endregion


        #region === Perceptor Methods ===
        // Returns true if at the coordinates, on the board, there is no Piece
        public bool IsFree(Vector2Int coords)
        {
            return !this.Board.OutOfBounds(coords) && this.Board.GetPiece(coords) == null;
        }

        public List<Piece> PiecesInSight(float range)
        {
            float toExplore = range;

            List<Piece> pieces = new List<Piece>();

            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(this.Coords);

            Dictionary<Vector2Int, LevelBoard.Directions> toExpand = new Dictionary<Vector2Int, LevelBoard.Directions>
            {
                { this.Coords, LevelBoard.Directions.None }
            };

            Queue<Vector2Int> nextQueue;
            Dictionary<Vector2Int, LevelBoard.Directions> toExpandNext;

            while (toExplore > 0f)
            {
                nextQueue = new Queue<Vector2Int>();
                toExpandNext = new Dictionary<Vector2Int, LevelBoard.Directions>();

                while (queue.Count > 0)
                {
                    Vector2Int coords = queue.Dequeue();
                    if (!toExpand.TryGetValue(coords, out LevelBoard.Directions originDir)) continue;
                    toExpand.Remove(coords);

                    int dirId = (int)originDir;

                    Vector2Int adjCoords;
                    Piece found;

                    switch (originDir)
                    {
                        case LevelBoard.Directions.None:
                            for (int i = 0; i < 8; i++)
                            {
                                LevelBoard.Directions checkDir = (LevelBoard.Directions)(i % 8);
                                adjCoords = this.Board.GetAdjacentCoords(coords, checkDir);

                                // Out of Bounds
                                if (adjCoords.x == -1) continue;

                                if (i % 2 == 0 && toExplore - 1 >= 0f ||
                                    i % 2 == 1 && toExplore - 1 >= 0.5f)
                                {
                                    nextQueue.Enqueue(adjCoords);          // Queue position to be checked
                                    toExpandNext.Add(adjCoords, checkDir); // List direction it was reached from
                                }

                                found = this.Board.GetPiece(adjCoords);

                                // No Piece in the space
                                if (found == null) continue;

                                pieces.Add(found); // Add Piece to the list
                            }
                            break;

                        case LevelBoard.Directions.East:
                        case LevelBoard.Directions.South:
                        case LevelBoard.Directions.West:
                        case LevelBoard.Directions.North:
                            for (int i = dirId - 1; i <= dirId + 1; i++)
                            {
                                LevelBoard.Directions checkDir = (LevelBoard.Directions)(i % 8);
                                if (checkDir == LevelBoard.Directions.None) checkDir = LevelBoard.Directions.SouthEast;

                                // Get adjacent space coordinates
                                adjCoords = this.Board.GetAdjacentCoords(coords, checkDir);

                                // Out of Bounds
                                if (adjCoords.x == -1) continue;

                                if (toExplore - 1 >= 0f)
                                {
                                    nextQueue.Enqueue(adjCoords);          // Queue position to be checked
                                    toExpandNext.Add(adjCoords, checkDir); // List direction it was reached from
                                }

                                found = this.Board.GetPiece(adjCoords);

                                // No Piece in the space
                                if (found == null) continue;

                                pieces.Add(found); // Add Piece to the list
                            }
                            break;

                        case LevelBoard.Directions.SouthEast:
                        case LevelBoard.Directions.SouthWest:
                        case LevelBoard.Directions.NorthEast:
                        case LevelBoard.Directions.NorthWest:
                            adjCoords = this.Board.GetAdjacentCoords(coords, originDir);

                            // Out of Bounds
                            if (adjCoords.x == -1) continue;

                            if (toExplore - 1 >= 0.5f)
                            {
                                nextQueue.Enqueue(adjCoords);           // Queue position to be checked
                                toExpandNext.Add(adjCoords, originDir); // List direction it was reached from
                            }

                            found = this.Board.GetPiece(adjCoords);

                            // No Piece in the space
                            if (found == null) continue;

                            pieces.Add(found); // Add Piece to the list
                            break;

                    }
                }

                queue    = nextQueue;
                toExpand = toExpandNext;

                toExplore--;
            }

            return pieces;
        }
        #endregion


        #region === State Methods ===
        public void SaveState()
        {
            var state = new AgentState(this.Coords, this.Stats, this.Orientation, this.ChosenAction);

            // If the a log of this turn already exists
            if (this.StateLogExists(this.Turn))
            {
                // Replace it
                this.StateLog[this.Turn] = state;
            }

            // If there is none
            else
            {
                this.StateLog.Add(state);
            }
        }

        protected bool StateLogExists(int turn)
        {
            var index = turn - this.StartTurn;

            if (this.StateLog.Count > index)
            {
                return true;
            }

            return false;
        }

        protected void LoadState()
        {
            var state = this.StateLog[this.Turn];

            this.Board.MovePiece(state.Coords, this);

            this.transform.position = LevelBoard.WorldCoords(this.Coords);

            this.Stats = state.Stats;

            this.Orientation = state.Orientation;

            this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, ((int) this.Orientation) * 45, this.transform.eulerAngles.z);

            this.ChosenAction = state.ChosenAction;
        }
        #endregion
    }
}