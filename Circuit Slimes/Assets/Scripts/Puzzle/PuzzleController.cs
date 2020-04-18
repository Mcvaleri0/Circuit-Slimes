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
        public const string LEVELS_PATH = "./Assets/Resources/Levels";

        public Puzzle Puzzle { get; private set; }

        public int CurrentLevel { get; private set; }
        public int nLevels;


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

        private int CurrentAgent;

        // Start is called before the first frame update
        void Start()
        {
            this.CurrentLevel = 0;

            this.State = RunState.Idle;

            this.Turn = 0;

            this.LoadPuzzle(this.CurrentLevel);

            // TODO: remove this when creator mode is able to choose level
            CreatorController create = GameObject.Find("CreatorController").GetComponent<CreatorController>();
            create.Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.P)) this.SavePuzzle(1);

            switch(this.State)
            {
                default:
                    break;

                case RunState.Idle:
                    if (this.GoalTurn > this.Turn)
                    {
                        this.State = RunState.StepForward;
                    }
                    else if (this.GoalTurn < this.Turn)
                    {
                        this.CurrentAgent = this.Puzzle.Agents.Count - 1;
                        this.State = RunState.StepBack;
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


        #region === Simulation Aux Methods ===
        public bool RunAgents()
        {
            if(this.CurrentAgent < this.Puzzle.Agents.Count)
            {
                Agent agent = this.Puzzle.Agents[this.CurrentAgent];

                if (agent.Turn > this.Turn)
                {
                    this.CurrentAgent++;

                    return false;
                }

                if(!agent.Active)
                {
                    agent.SaveState();

                    agent.Turn++;

                    this.CurrentAgent++;

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
                this.CurrentAgent = 0;
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
            this.Turn = 0;
            this.GoalTurn = 0;
            this.CurrentAgent = 0;

            foreach(var piece in this.Puzzle.Pieces)
            {
                if(piece is Agent agent)
                {
                    agent.State = Agent.States.Restart;
                }
            }
        }
        #endregion


        #region === Save and Load Functions ===
        public void SavePuzzle(int level)
        {
            Debug.Log("Saving level " + level + ".");

            //level = 1;

            //LevelBoard b = new LevelBoard();
            //b.Initialize(8,8);

            //this.Puzzle = new Puzzle();
            //this.Puzzle.Initialize(b);

            //Piece p;
            //var positions = new ArrayList();
            //positions.Add(new Vector2Int(0, 0));
            ////positions.Add(new Vector2(0, 1));
            ////positions.Add(new Vector2(1, 0));
            ////positions.Add(new Vector2(1, 1));

            //for (int i = 0; i < 1; i++)
            //{
            //    p = new Piece();
            //    p.Initialize(b, (Vector2Int) positions[i], Piece.SlimeTypes.Electric);
            //    this.Puzzle.AddPiece(p);
            //}

            //Tile t;
            //positions = new ArrayList();
            //positions.Add(new Vector2Int(0, 0));
            //positions.Add(new Vector2Int(0, 1));
            //positions.Add(new Vector2Int(0, 2));
            //positions.Add(new Vector2Int(1, 2));
            //positions.Add(new Vector2Int(2, 2));

            //for (int i = 0; i < 5; i++)
            //{
            //    t = new Tile();
            //    t.Initialize(b, (Vector2Int) positions[i], Tile.Types.Solder);
            //    this.Puzzle.AddTile(t);
            //}

            // FIXME: not sure if this works on a phone
            // the exemple used Application.persistentDataPath
            // yeah... this doesn't work on a phone. we need to use the persistentdatapath
            PuzzleData puzzleData = new PuzzleData(this.Puzzle);
            puzzleData.Save(LEVELS_PATH, "Level" + level);

            Debug.Log("Puzzle Saved. Wait for the file to update");
        }

        public void LoadPuzzle(int level)
        {
            // this needs to be like this because UnityEngine overrides != == operators
            // because of that null and "null" exist. when using the operators, 
            // although "null" is not really null it behaves as such
            if (!object.Equals(this.Puzzle, null))
            {
                this.Puzzle.Destroy();
            }

            this.Puzzle = PuzzleData.Load(LEVELS_PATH, "Level" + level);

            Debug.Log("Puzzle Loaded");
        }

        public void ClearPuzzle()
        {
            this.Puzzle.Destroy();

            this.Puzzle = null;
        }
        #endregion


        #region === Level Control Functions ===

        public void NextLevel()
        {
            this.State = RunState.Idle;

            this.CurrentLevel = (this.CurrentLevel + 1) % this.nLevels;

            this.LoadPuzzle(this.CurrentLevel);
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

            this.LoadPuzzle(this.CurrentLevel);
            this.Restart();
        }

        #endregion
    }
}
