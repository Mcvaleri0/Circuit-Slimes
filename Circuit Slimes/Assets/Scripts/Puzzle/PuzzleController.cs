using Puzzle.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Puzzle.Pieces;
using Creator;



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
            StepForward,
            StepBack,
            Done
        }

        private RunState State;

        private int Turn     =   0;
        private int MaxTurn  = 250;
        private int GoalTurn =   0;

        private int CurrentAgent  = 0;
        private int StoppedAgents = 0;
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

        // Update is called once per frame
        void Update()
        {
            switch(this.State)
            {
                default:
                    break;

                case RunState.Idle:
                    if (this.GoalTurn > this.Turn &&
                        this.StoppedAgents < this.Puzzle.Agents.Count)
                    {
                        this.StoppedAgents = 0;
                        this.State = RunState.StepForward;
                    }
                    else if (this.GoalTurn < this.Turn)
                    {
                        this.CurrentAgent = this.Puzzle.Agents.Count - 1;
                        this.State = RunState.StepBack;
                    }

                    if(this.StoppedAgents >= this.Puzzle.Agents.Count)
                    {
                        if (this.WinCondition.Equals(null)) break;

                        if(this.WinCondition.Verify(this.Puzzle))
                        {
                            Debug.Log("WIN");
                        }
                    }
                    break;

                case RunState.StepForward:
                    if (this.RunAgents())
                    {
                        this.State = RunState.Idle;
                        this.Turn++;
                    }   
                    break;

                case RunState.StepBack:
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
        
        public bool RunAgents()
        {
            if(this.CurrentAgent < this.Puzzle.Agents.Count)
            {
                Agent agent = this.Puzzle.Agents[this.CurrentAgent];

                if (agent.Turn > this.Turn)
                {
                    this.CurrentAgent++;

                    if (agent.NoAction) this.StoppedAgents++;

                    return false;
                }

                if(!agent.Active)
                {
                    agent.Turn++;

                    this.CurrentAgent++;

                    this.StoppedAgents++;

                    return false;
                }

                if (agent.State == Agent.States.Idle ||
                    agent.State == Agent.States.Waiting)
                {
                    agent.State = Agent.States.Think;
                }
            }
            else
            {
                this.CurrentAgent = 0;
                return true;
            }

            return false;
        }

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
