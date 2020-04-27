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
        #region /* Level Attributes */

        private const string LEVELS_PATH = "Levels";
        private const string PLAYERS_LEVEL_NAME = "LevelPlayer";
        private const string EMPTY_LEVEL_NAME = "newLevel";

        public const int PLAYERS_LEVEL = -1;
        public const int EMPTY_LEVEL   = -2;

        // set on editor
        public int CurrentLevel;
        public int nLevels;

        #endregion

        #region /* Puzzle Attibutes */

        public Puzzle Puzzle { get; private set; }

        private WinCondition WinCondition { get; set; }
        #endregion

        #region /* Creator Attributes */

        private CreatorController CreatorController { get; set; }

        // set on editor
        public bool Creator;

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


        
        #region === Unity Events ===

        // Start is called before the first frame update
        void Start()
        {
            this.State = RunState.Idle;

            this.Turn = 0;

            this.LoadLevel(this.CurrentLevel);

            this.CreatorController = GameObject.Find("CreatorController").GetComponent<CreatorController>();
            this.CreatorController.Initialize(this, this.Puzzle, this.Creator);
        }

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
                        else
                        {
                            Debug.Log("Fail");
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


        #region === Level Functions ===

        public void LoadLevel(int level)
        {
            // this needs to be like this because UnityEngine overrides != == operators
            // because of that null and "null" exist. when using the operators, 
            // although "null" is not really null it behaves as such
            if (!object.Equals(this.Puzzle, null))
            {
                this.Puzzle.Destroy();
            }

            this.CurrentLevel = level;

            string path;
            string name;

            if (level == PLAYERS_LEVEL)
            {
                if (this.CreatePlayersLevel())
                {
                    path = Path.Combine(Application.streamingAssetsPath, LEVELS_PATH);
                    name = EMPTY_LEVEL_NAME;
                }
                else
                {
                    path = Path.Combine(Application.persistentDataPath, LEVELS_PATH);
                    name = PLAYERS_LEVEL_NAME;
                }
            }
            else
            {
                path = Path.Combine(Application.streamingAssetsPath, LEVELS_PATH);
                name = "Level" + level;
            }

            this.Puzzle = PuzzleData.Load(path, name);

            this.WinCondition = this.Puzzle.WinCondition;

            Debug.Log("Puzzle Loaded");
        }

        public void SaveLevel(int level)
        {
            Debug.Log("Saving level " + level + ".");

            string path;
            string name;

            if (level == PLAYERS_LEVEL)
            {
                path = Path.Combine(Application.persistentDataPath, LEVELS_PATH);
                name = PLAYERS_LEVEL_NAME;
            }
            else
            {
                path = Path.Combine(Application.streamingAssetsPath, LEVELS_PATH);
                name = "Level" + level;
            }


            PuzzleData puzzleData = new PuzzleData(this.Puzzle);
            puzzleData.Save(path, name);

            Debug.Log("Puzzle Saved. Wait for the file to update");
        }

        private bool CreatePlayersLevel()
        {
            string path = Path.Combine(Application.persistentDataPath, LEVELS_PATH);
            string completePath = Path.Combine(path, PLAYERS_LEVEL_NAME + ".json");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                File.Create(completePath);
                return true;
            }
            else if (!File.Exists(completePath))
            {
                File.Create(completePath);
                return true;
            }

            return false;
        }

        public void NextLevel()
        {
            this.State = RunState.Idle;

            this.CurrentLevel = (this.CurrentLevel + 1) % this.nLevels;

            this.LoadLevel(this.CurrentLevel);

            this.CreatorController.UpdateInfo(this.Puzzle);
            this.Restart();
        }

        public void PreviousLevel()
        {
            this.State = RunState.Idle;

            this.CurrentLevel = (this.CurrentLevel - 1) % this.nLevels;

            if (this.CurrentLevel < 0)
            {
                this.CurrentLevel += this.nLevels;
            }

            this.LoadLevel(this.CurrentLevel);

            this.CreatorController.UpdateInfo(this.Puzzle);
            this.Restart();
        }

        public void ClearPuzzle()
        {
            this.Puzzle.Destroy();

            this.Puzzle = null;
        }

        #endregion

    }
}
