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
            Act,
            Rewind,
            Restart,
            Waiting
        };

        public States State;

        protected List<Action> KnownActions;

        public Action ChosenAction { get; protected set; }

        public bool NoAction { get; protected set; }

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

        public Dictionary<int, Action> ActionLog;

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
        new protected virtual void Update()
        {
            switch (this.State)
            {
                default:
                case States.Idle:
                case States.Waiting:
                    break;

                case States.Think:
                    this.NoAction = false;

                    // Decide on an Action
                    this.ChosenAction = Think();

                    if (this.ChosenAction == null)
                    {
                        this.State = States.Waiting;

                        this.NoAction = true;

                        this.Turn++;
                    }
                    else
                    {
                        this.ActionLog.Add(this.Turn, this.ChosenAction);

                        // Go to Act
                        this.State = States.Act;
                    }
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

                case States.Restart:
                    this.Turn = this.StartTurn;

                    this.ActionLog.Clear();

                    this.State = States.Idle;
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

            var coors = this.Coords;

            this.Board.RemovePiece(this);

            this.Coords = coors;

            this.transform.position = new Vector3(this.transform.position.x, -2, this.transform.position.z);
        }

        // Reactivates the Agent
        public void Reactivate(Vector2Int coords)
        {
            this.Active = true;

            this.Board.PlacePiece(coords, this);

            this.transform.position = LevelBoard.WorldCoords(coords);

            this.Turn++;
        }
        #endregion


        #region === Actuator Methods ===
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

        virtual public bool Move(Vector2Int targetCoords)
        {
            var targetPosition = LevelBoard.WorldCoords(targetCoords);

            var maxVelocity = this.Stats.Speed / 100f;

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

                this.Board.MovePiece(targetCoords, this);

                return true;
            }
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

                                found = this.Board.GetPiece(adjCoords);

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
    }
}