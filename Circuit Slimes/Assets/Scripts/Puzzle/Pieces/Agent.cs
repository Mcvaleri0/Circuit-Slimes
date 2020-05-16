using UnityEngine;
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
            ReadyToAct,
            ConfirmAction,
            Act,
            NoAction,
            Rewind,
            Restart,
            Waiting
        };

        public States State;

        #region Actions
        protected List<Action> KnownActions;

        public Action ChosenAction { get; protected set; }

        public Dictionary<int, Action> ActionLog;
        #endregion

        #region Stats
        public struct Statistics
        {
            public int MaxHP, HP, Speed, MaxFood, Food;

            public Statistics(int maxhp, int speed, int maxfood)
            {
                this.MaxHP = maxhp;
                this.HP = maxhp;

                this.Speed = speed;

                this.MaxFood = maxfood;
                this.Food = 0;
            }
        }

        public Statistics Stats;
        #endregion


        public bool Active { get; protected set; }

        // Init Method
        virtual public void Initialize(Puzzle puzzle, Vector2Int coords, Caracteristics caracterization,
            LevelBoard.Directions ori = 0, int turn = 0)
        {
            base.Initialize(puzzle, coords, caracterization);

            this.State = States.Idle;

            this.KnownActions = new List<Action>();

            this.ActionLog = new Dictionary<int, Action>();

            this.Orientation = ori;

            this.StartTurn = turn;
            this.Turn = turn;
        }


        #region === Unity Methods ===
        // Use this for initialization
        new protected virtual void Start()
        {
            this.State = States.Idle;

            this.Active = true;
        }

        // Update is called once per frame
        override protected void Update()
        {
            base.Update();

            switch (this.State)
            {
                default:
                case States.Idle:
                case States.Waiting:
                case States.NoAction:
                case States.ReadyToAct:
                    break;

                case States.Think:
                    // Decide on an Action
                    this.ChosenAction = Think();

                    if (this.ChosenAction == null)
                    {
                        this.State = States.NoAction;
                    }
                    else
                    {
                        this.ActionLog.Add(this.Turn, this.ChosenAction);

                        // Go to Act
                        this.State = States.ReadyToAct;
                    }
                    break;

                case States.ConfirmAction:
                    if(this.ChosenAction.Confirm(this))
                    {
                        this.ChosenAction.Begin(this);

                        this.State = States.Act;
                    }
                    else
                    {
                        this.ActionLog.Remove(this.Turn);

                        this.ChosenAction = null;

                        this.Turn++;

                        this.State = States.Waiting;
                    }
                    break;

                case States.Act:
                    // If the Action has been completed
                    if (this.ChosenAction.Execute(this))
                    {
                        this.ChosenAction.End(this);

                        this.ChosenAction = null;

                        this.Turn++;

                        // Go to waiting
                        this.State = States.Waiting;
                    }
                    break;

                case States.Rewind:
                    this.Turn--;

                    if(this.ActionLog.TryGetValue(this.Turn, out Action prevAction))
                    {
                        this.ChosenAction = prevAction;

                        this.ActionLog.Remove(this.Turn);

                        this.ChosenAction.Undo(this);
                    }
                    else
                    {
                        this.ChosenAction = null;
                    }

                    this.State = States.Waiting;
                    break;
            }
        }
        #endregion


        #region === Agent Methods ===
        // Returns the Chosen Action on a given Turn
        public virtual Action Think()
        {
            foreach (var action in this.KnownActions)
            {
                Action result = action.Available(this);

                if (result != null) return result;
            }

            return null;
        }

        // Deactivates the Agent
        public void Deactivate()
        {
            this.Active = false;

            var coords = this.Coords;

            this.RemovePiece(this.Coords);

            this.Coords = coords;

            this.Hide();
        }

        // Reactivates the Agent
        public void Reactivate(Vector2Int coords)
        {
            this.Reveal();

            this.PlacePiece(this, coords);

            this.transform.position = LevelBoard.WorldCoords(coords);

            this.Active = true;
        }
        #endregion


        #region === Actuator Methods ===
        #region Self
        // World
        virtual public bool Rotate(LevelBoard.Directions targetDir, float percentage = 0.33f)
        {
            float currentAngle = this.transform.eulerAngles.y;
            float targetAngle  = 360 - ((float) targetDir) * 45f;
            if (targetAngle == 360) targetAngle = 0;

            currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, percentage);

            if (Mathf.Abs((currentAngle % 360) - targetAngle) < 0.1f) currentAngle = targetAngle;

            this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, currentAngle, this.transform.eulerAngles.z);

            if (currentAngle == targetAngle || currentAngle - targetAngle == 360)
            {
                // Update their Orientation
                this.Orientation = targetDir;

                return true;
            }

            return false;
        }

        virtual public bool Move(Vector2Int targetCoords, float minSpeed = 0f)
        {
            var targetPosition = LevelBoard.WorldCoords(targetCoords);

            var speed = Mathf.Max(minSpeed, this.Stats.Speed);

            var maxVelocity = speed / 100f;

            var currentPosition = this.transform.position;

            // If the distance to the Target Position exceeds what can be traveled in one Step
            if (Vector3.Distance(currentPosition, targetPosition) > maxVelocity)
            {
                var dX = targetPosition.x - currentPosition.x; // Distance to travel (North - South)
                var dZ = targetPosition.z - currentPosition.z; // Distance to travel (West  - East)

                var norm = (new Vector3(dX, 0, dZ)).normalized; // Normalize the distance vector;

                this.transform.position += norm * maxVelocity; // Apply movement

                return false;
            }

            // If the Agent is within a one Step distance of the Target Position
            else
            {
                this.transform.position = targetPosition; // Set their position to the Target Position

                return true;
            }
        }

        // Board
        virtual public bool RotateInBoard(LevelBoard.Directions orientation)
        {
            return true;
        }
        
        virtual public bool MoveInBoard(Vector2Int coords)
        {
            return this.Puzzle.MovePiece(coords, this);
        }
        #endregion


        #region Create and Destroy
        virtual public Piece CreatePiece(Caracteristics caracterization, Vector2Int coords,
            LevelBoard.Directions ori = LevelBoard.Directions.East, int turn = 0)
        {
            return this.Puzzle.CreatePiece(caracterization, coords, ori, turn);
        }

        virtual public Tile CreateTile(Tile.Types type, Vector2Int coords)
        {
            return this.Puzzle.CreateTile(type, coords);
        }

        virtual public bool DestroyPiece(Piece piece)
        {
            return this.Puzzle.RemovePiece(piece);
        }

        virtual public bool DestroyTile(Tile tile)
        {
            return this.Puzzle.RemoveTile(tile);
        }
        #endregion


        #region Place and Remove
        virtual public bool PlacePiece(Piece piece, Vector2Int coords)
        {
            return this.Puzzle.Board.PlacePiece(piece, coords);
        }


        virtual public bool RemovePiece(Vector2Int coords)
        {
            var piece = this.PieceAt(coords);

            if(piece != null) return this.Puzzle.Board.RemovePiece(piece);

            return false;
        }
        

        virtual public bool RemoveTile(Vector2Int coords)
        {
            var tile = this.TileAt(coords);

            if(tile != null)
            {
                return this.Puzzle.RemoveTile(tile);
            }

            return false;
        }
        #endregion
        #endregion


        #region === Perceptor Methods ===
        // Returns true if at the coordinates, on the board, there is no Piece
        public bool IsFree(Vector2Int coords)
        {
            return !this.Puzzle.OutOfBounds(coords) && this.Puzzle.GetPiece(coords) == null;
        }

        
        public bool CanMove(Vector2Int coords)
        {
            return this.Puzzle.CanPlacePiece(coords, this);
        }

        public bool CanRotate(LevelBoard.Directions orientation)
        {
            return this.Puzzle.CanPlacePiece(this.Coords, orientation, this);
        }

        public bool CanMoveAndRotate(Vector2Int coords, LevelBoard.Directions orientation)
        {
            return this.Puzzle.CanPlacePiece(coords, orientation, this);
        }


        public Piece PieceAt(Vector2Int coords)
        {
            return this.Puzzle.GetPiece(coords);
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
                                adjCoords = LevelBoard.GetAdjacentCoords(coords, checkDir);

                                // Out of Bounds
                                if (adjCoords.x == -1) continue;

                                found = this.Puzzle.GetPiece(adjCoords);

                                // No Piece in the space
                                if (found == null)
                                {
                                    if (i % 2 == 0 && toExplore - 1 >= 0f ||
                                        i % 2 == 1 && toExplore - 1 >= 0.5f)
                                    {
                                        nextQueue.Enqueue(adjCoords);          // Queue position to be checked
                                        toExpandNext.Add(adjCoords, checkDir); // List direction it was reached from
                                    }

                                    continue;
                                }

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
                                adjCoords = LevelBoard.GetAdjacentCoords(coords, checkDir);

                                // Out of Bounds
                                if (adjCoords.x == -1) continue;

                                if (toExplore - 1 >= 0f)
                                {
                                    nextQueue.Enqueue(adjCoords);          // Queue position to be checked
                                    toExpandNext.Add(adjCoords, checkDir); // List direction it was reached from
                                }

                                found = this.Puzzle.GetPiece(adjCoords);

                                // No Piece in the space
                                if (found == null) continue;

                                pieces.Add(found); // Add Piece to the list
                            }
                            break;

                        case LevelBoard.Directions.SouthEast:
                        case LevelBoard.Directions.SouthWest:
                        case LevelBoard.Directions.NorthEast:
                        case LevelBoard.Directions.NorthWest:
                            adjCoords = LevelBoard.GetAdjacentCoords(coords, originDir);

                            // Out of Bounds
                            if (adjCoords.x == -1) continue;

                            if (toExplore - 1 >= 0.5f)
                            {
                                nextQueue.Enqueue(adjCoords);           // Queue position to be checked
                                toExpandNext.Add(adjCoords, originDir); // List direction it was reached from
                            }

                            found = this.Puzzle.GetPiece(adjCoords);

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

        public bool PieceExists(Piece piece)
        {
            return this.Puzzle.Pieces.Contains(piece);
        }


        public Tile TileAt(Vector2Int coords)
        {
            return this.Puzzle.GetTile(coords);
        }
        #endregion
    }
}