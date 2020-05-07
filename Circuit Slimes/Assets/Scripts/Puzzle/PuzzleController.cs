using Puzzle.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Puzzle.Pieces;
using Game;



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

        private int Turn     =   0;
        private int MaxTurn  = 250;
        private int GoalTurn =   0;

        private int CurrentAgent  = 0;
        private int StoppedAgents = 0;

        private List<Agent> Agents;
        #endregion


        #region /* Game Attributes */

        private GameController GameController { get; set; }

        #endregion



        #region === Init / Update Puzzle Info ===

        public void Initialize(Puzzle puzzle)
        {
            this.State = RunState.Idle;

            this.Turn = 0;

            this.Puzzle = puzzle;
            this.WinCondition = this.Puzzle.WinCondition;
        }


        public void UpdatePuzzle(Puzzle puzzle)
        {
            this.State = RunState.Idle;

            this.Puzzle = puzzle;
            this.WinCondition = this.Puzzle.WinCondition;

            this.Restart();
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
                    foreach(var agent in this.Agents)
                    {
                        if(agent.State == Agent.States.ReadyToAct)
                        {
                            agent.State = Agent.States.ConfirmAction;
                        }
                    }

                    this.State = RunState.AwaitConclusion;
                    break;

                case RunState.AwaitConclusion:
                    foreach(var agent in this.Agents)
                    {
                        if (agent.State != Agent.States.Waiting) return;
                    }

                    this.Turn++;

                    this.State = RunState.Idle;
                    break;

                case RunState.Rewinding:
                    if (this.RewindAgents())
                    {
                        this.State = RunState.Idle;
                        this.Turn--;
                    }
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


        //public void NextLevel()
        //{
        //    this.State = RunState.Idle;

        //    this.CurrentLevel = (this.CurrentLevel + 1) % this.nLevels;

        //    this.LoadLevel(this.CurrentLevel);

        //    this.CreatorController.UpdateInfo(this.Puzzle);
        //    this.Restart();
        //}

        //public void PreviousLevel()
        //{
        //    this.State = RunState.Idle;

        //    this.CurrentLevel = (this.CurrentLevel - 1) % this.nLevels;

        //    if (this.CurrentLevel < 0)
        //    {
        //        this.CurrentLevel += this.nLevels;
        //    }

        //    this.LoadLevel(this.CurrentLevel);

        //    this.CreatorController.UpdateInfo(this.Puzzle);
        //    this.Restart();
        //}

    }
}
