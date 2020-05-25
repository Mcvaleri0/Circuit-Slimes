using Puzzle.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Puzzle.Pieces;
using Game;
using Puzzle.Tiles;
using Puzzle.Pieces.Slimes;
using Puzzle.Pieces.Components;

namespace Puzzle
{
    public class PuzzleController : MonoBehaviour
    {
        #region /* Puzzle Attibutes */

        public Puzzle Puzzle { get; private set; }

        private WinCondition WinCondition { get; set; }

        #endregion


        #region /* Simulation Attributes */
        private enum RunState
        {
            Idle,
            Deciding,
            BeginActing,
            AwaitConclusion,
            Rewinding,
            Done
        }

        [SerializeField]
        private RunState State;

        [SerializeField]
        private int Turn     =   0;
        private int MaxTurn  = 250;
        private int GoalTurn =   0;

        private int CurrentAgent  = 0;
        [SerializeField]
        private int StoppedAgents = 0;

        private List<Agent> Agents;
        #endregion


        #region /* Game Attributes */

        private GameController GameController { get; set; }

        #endregion


        #region /* AASMA Analytics */
        public bool AASMAMetrics = false;

        private List<int> ReachedTilesPerTurn;

        private List<SolderTile> Unreached;

        private int TileCount;
        #endregion


        #region === Init / Update Puzzle Info ===

        public void Initialize(Puzzle puzzle)
        {
            this.State = RunState.Idle;

            this.Turn = 0;

            this.Puzzle = puzzle;
            this.WinCondition = this.Puzzle.WinCondition;

            if(this.AASMAMetrics) this.InitAASMAMetrics();
        }


        public void UpdatePuzzle(Puzzle puzzle)
        {
            this.State = RunState.Idle;

            this.Puzzle = puzzle;
            this.WinCondition = this.Puzzle.WinCondition;

            this.Turn = 0;
            this.StoppedAgents = 0;

            if (this.AASMAMetrics)  this.InitAASMAMetrics();

            this.Restart();
        }


        private void InitAASMAMetrics()
        {
            this.ReachedTilesPerTurn = new List<int>();

            this.Unreached = new List<SolderTile>();

            foreach (var tile in this.Puzzle.Tiles)
            {
                if (tile is SolderTile tl &&
                   !(this.Puzzle.GetPiece(tl.Coords) is CircuitComponent))
                {
                    this.Unreached.Add(tl);
                }
            }

            this.TileCount = this.Unreached.Count;
        }
        #endregion


        #region === Unity Events ===

        private void Awake()
        {
            this.GameController = GameController.CreateGameController();
        }


        // Update is called once per frame
        void Update()
        {
            if (this.AASMAMetrics && Input.GetKeyUp(KeyCode.Backspace))
            {
                this.CalculateExplorationRate();

                var explored = 1 - (float) this.Unreached.Count / (float) this.TileCount;

                Debug.Log("Explored: " + explored);
            }

            switch(this.State)
            {
                default:
                    break;

                case RunState.Idle:
                    #region
                    if (this.GoalTurn > this.Turn &&
                        this.StoppedAgents < this.Puzzle.Agents.Count)
                    {
                        this.StoppedAgents = 0;
                        this.Agents = new List<Agent>(this.Puzzle.Agents);
                        this.State = RunState.Deciding;
                    }
                    else if (this.GoalTurn < this.Turn)
                    {
                        this.StoppedAgents = 0;
                        this.CurrentAgent = this.Puzzle.Agents.Count - 1;
                        this.State = RunState.Rewinding;
                    }

                    if(this.Agents != null && this.StoppedAgents >= this.Agents.Count)
                    {
                        if (this.WinCondition.Equals(null)) break;

                        if(this.WinCondition.Verify(this.Puzzle))
                        {
                            Debug.Log("WIN");
                        }
                    }
                    #endregion
                    break;

                case RunState.Deciding:
                    #region
                    if(this.CurrentAgent < this.Agents.Count)
                    {
                        var agent = this.Agents[this.CurrentAgent];

                        // If the Agent has been Deactivated
                        if(!agent.Active)
                        {
                            agent.Turn++;

                            this.CurrentAgent++;

                            this.StoppedAgents++;
                        }

                        else
                        {
                            switch(agent.State)
                            {
                                default:
                                    break;

                                case Agent.States.Idle:
                                case Agent.States.Waiting:
                                    agent.State = Agent.States.Think;
                                    break;

                                case Agent.States.ReadyToAct:
                                    this.CurrentAgent++;
                                    break;

                                case Agent.States.NoAction:
                                    agent.Turn++;

                                    agent.State = Agent.States.Waiting;

                                    this.CurrentAgent++;

                                    this.StoppedAgents++;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        this.CurrentAgent = 0;
                        this.State = RunState.BeginActing;
                    }
                    #endregion
                    break;

                case RunState.BeginActing:
                    #region
                    if(this.StoppedAgents >= this.Agents.Count)
                    {
                        foreach(var agent in this.Agents)
                        {
                            agent.Turn = this.Turn;
                        }

                        this.State = RunState.Idle;

                        return;
                    }

                    foreach(var agent in this.Agents)
                    {
                        if(agent.State == Agent.States.ReadyToAct)
                        {
                            agent.State = Agent.States.ConfirmAction;
                        }
                    }

                    this.State = RunState.AwaitConclusion;
                    #endregion
                    break;

                case RunState.AwaitConclusion:
                    #region
                    foreach(var agent in this.Agents)
                    {
                        if (agent.State != Agent.States.Waiting) return;
                    }

                    if(this.AASMAMetrics) this.CalculateReachedTiles();

                    this.Turn++;

                    this.State = RunState.Idle;
                    #endregion
                    break;

                case RunState.Rewinding:
                    #region
                    if (this.RewindAgents())
                    {
                        this.State = RunState.Idle;

                        if (this.AASMAMetrics) this.ReachedTilesPerTurn.RemoveAt(this.Turn - 1);

                        this.Turn--;

                        if (this.Turn == 0)
                        {
                            this.GameController.RewindFinished();
                        }
                    }
                    #endregion
                    break;
            }
        }

        #endregion


        #region === Simulation Aux Methods ===
        public bool RewindAgents()
        {
            if (this.CurrentAgent >= 0)
            {
                Agent agent = this.Puzzle.Agents[this.CurrentAgent];

                if (agent.Turn < this.Turn)
                {
                    this.CurrentAgent--;

                    return false;
                }

                agent.State = Agent.States.Rewind;
            }
            else
            {
                this.CurrentAgent  = 0;
                this.StoppedAgents = 0;
                return true;
            }

            return false;
        }
        
        #endregion


        #region === Simulation Control Functions ===
        
        public void Play()
        {
            this.GoalTurn = this.MaxTurn;
        }

        public void Pause()
        {
            if(this.Turn + 1 < this.GoalTurn)
            {
                this.GoalTurn = this.Turn + 1;
            }
        }

        public void StepForward()
        {
            if (this.StoppedAgents >= this.Puzzle.Agents.Count) return;

            this.GoalTurn = this.Turn + 1;
            this.GoalTurn = Mathf.Min(this.GoalTurn, this.MaxTurn);
        }

        public void StepBack()
        {
            this.GoalTurn = this.Turn - 1;
            this.GoalTurn = Mathf.Max(this.GoalTurn, 0);
        }

        public void Restart()
        {
            this.State = RunState.Idle;
            this.GoalTurn = 0;
        }

        #endregion


        #region === AASMA Analytics ===
        private void CalculateReachedTiles()
        {
            var count = 0;

            var nextUnreached = new List<SolderTile>();

            foreach(var tile in this.Unreached)
            {
                var piece = this.Puzzle.GetPiece(tile.Coords);

                if (piece is ElectricSlime)
                {
                    count++;
                }
                else
                {
                    nextUnreached.Add(tile);
                }
            }

            this.Unreached = nextUnreached;

            this.ReachedTilesPerTurn.Add(count);
        }

        private void CalculateExplorationRate()
        {
            float sum = 0;

            foreach(var val in this.ReachedTilesPerTurn)
            {
                sum += val;
            }

            Debug.Log("Exploration rate: " + (sum / (float) this.Turn));
        }
        #endregion
    }
}
