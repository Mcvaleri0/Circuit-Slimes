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
            public Vector2 Coords { get; private set; }

            public Statistics Stats { get; private set; }

            public LevelBoard.Directions Orientation { get; private set; }

            public Action ChosenAction { get; private set; }

            public AgentState(Vector2 coords, Statistics stats, LevelBoard.Directions ori, Action chosen)
            {
                this.Coords = coords;

                this.Stats = stats;

                this.Orientation = ori;

                this.ChosenAction = chosen;
            }
        }

        protected List<AgentState> StateLog;


        // Init Method
        public virtual void Initialize(LevelBoard board, Vector2 coords, Categories category,
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
        protected virtual void Start()
        {
            this.State = States.Idle;
        }

        // Update is called once per frame
        override protected void Update()
        {
            switch (this.State)
            {
                default:
                case States.Idle:
                case States.Waiting:
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
        public bool IsFree(int x, int y)
        {
            return this.Board.GetPiece(x, y) == null;
        }
        #endregion


        #region === State Methods ===
        protected void SaveState()
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

            this.Board.MovePiece((int)state.Coords.x, (int)state.Coords.y, this);

            this.transform.position = LevelBoard.WorldCoords(this.Coords);

            this.Stats = state.Stats;

            this.Orientation = state.Orientation;

            this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, ((int) this.Orientation) * -45, this.transform.eulerAngles.z);

            this.ChosenAction = state.ChosenAction;
        }
        #endregion
    }
}