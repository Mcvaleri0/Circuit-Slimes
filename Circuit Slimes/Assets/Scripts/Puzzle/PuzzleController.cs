using Puzzle.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Puzzle.Board;


namespace Puzzle
{
    public class PuzzleController : MonoBehaviour
    {
        public const string LEVELS_PATH = "./Assets/Resources/Levels";

        private Puzzle Puzzle { get; set; }

        private int CurrentLevel { get; set; }
        public  int nLevels;

        private enum RunState
        {
            Start,
            Running,
            Waiting,
            StepForward,
            StepBack,
            Paused,
            Resetting,
            Done
        }

        private RunState State;

        private int Turn;


        // Start is called before the first frame update
        void Start()
        {
            this.CurrentLevel = 0;

            this.State = RunState.Start;

            this.Turn = 0;

            this.LoadPuzzle(this.CurrentLevel);
        }

        // Update is called once per frame
        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.P)) this.SavePuzzle(1);

            switch(this.State)
            {
                default:
                case RunState.Start:
                case RunState.Paused:
                case RunState.Done:
                    break;

                case RunState.Running:
                case RunState.StepForward:
                    foreach(var piece in this.Puzzle.Pieces)
                    {
                        if(piece is Pieces.Agent)
                        {
                            var agent = (Pieces.Agent) piece;

                            if(agent.State == Pieces.Agent.States.Idle ||
                               agent.State == Pieces.Agent.States.Waiting)
                            {
                                agent.State = Pieces.Agent.States.Think;
                            }
                            else if(agent.State == Pieces.Agent.States.Inactive)
                            {
                                agent.SaveState();

                                agent.Turn++;
                            }
                        }
                    }

                    this.Turn++;

                    if (this.State == RunState.Running)     this.State = RunState.Waiting;
                    if (this.State == RunState.StepForward) this.State = RunState.Paused;
                    break;

                case RunState.Waiting:
                    var done = true;

                    foreach (var piece in this.Puzzle.Pieces)
                    {
                        if (piece is Pieces.Agent agent)
                        {
                            if (agent.State != Pieces.Agent.States.Waiting)
                            {
                                done = false;
                                break;
                            }
                        }
                    }

                    if(done)
                    {
                        this.State = RunState.Running;
                    }
                    break;

                case RunState.StepBack:
                    this.Turn -= 1;

                    foreach (var piece in this.Puzzle.Pieces)
                    {
                        if(piece is Pieces.Agent)
                        {
                            var agent = (Pieces.Agent) piece;

                            agent.State = Pieces.Agent.States.Rewind;
                        }
                    }

                    this.State = RunState.Paused;
                    if (this.Turn == 0) this.State = RunState.Start;
                    break;

                case RunState.Resetting:
                    break;
            }
        }


        #region === Simulation Control Functions ===
        public void Play()
        {
            if(this.State == RunState.Start ||
               this.State == RunState.Paused)
            {
                this.State = RunState.Running;
            }
        }

        public void Pause()
        {
            if (this.State == RunState.Waiting)
            {
                this.State = RunState.Paused;
            }
            else if(this.State == RunState.Running)
            {
                this.State = RunState.StepForward;
            }
        }

        public void StepForward()
        {
            if (this.State == RunState.Start ||
               this.State == RunState.Paused)
            {
                this.State = RunState.StepForward;
            }
        }

        public void StepBack()
        {
            if (this.State == RunState.Done ||
               this.State == RunState.Paused)
            {
                this.State = RunState.StepBack;
            }
        }

        public void Restart()
        {
            this.State = RunState.Start;
            this.Turn = 0;

            foreach(var piece in this.Puzzle.Pieces)
            {
                if(piece is Pieces.Agent)
                {
                    var agent = (Pieces.Agent) piece;

                    agent.State = Pieces.Agent.States.Restart;
                }
            }
        }
        #endregion


        #region === Save and Load Functions ===
        public void SavePuzzle(int level)
        {
            Debug.Log("Saving level " + level + ".");

            //level = 1;

            LevelBoard b = new LevelBoard();
            b.Initialize(8,8);

            this.Puzzle = new Puzzle();
            this.Puzzle.Initialize(b);

            Piece p;
            var positions = new ArrayList();
            positions.Add(new Vector2Int(0, 0));
            //positions.Add(new Vector2(0, 1));
            //positions.Add(new Vector2(1, 0));
            //positions.Add(new Vector2(1, 1));

            for (int i = 0; i < 1; i++)
            {
                p = new Piece();
                p.Initialize(b, (Vector2Int) positions[i], Piece.SlimeTypes.Electric);
                this.Puzzle.AddPiece(p);
            }

            Tile t;
            positions = new ArrayList();
            positions.Add(new Vector2Int(0, 0));
            positions.Add(new Vector2Int(0, 1));
            positions.Add(new Vector2Int(0, 2));
            positions.Add(new Vector2Int(1, 2));
            positions.Add(new Vector2Int(2, 2));

            for (int i = 0; i < 5; i++)
            {
                t = new Tile();
                t.Initialize(b, (Vector2Int) positions[i], Tile.Types.Solder);
                this.Puzzle.AddTile(t);
            }

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
            this.State = RunState.Start;

            this.CurrentLevel = (this.CurrentLevel + 1) % this.nLevels;

            this.LoadPuzzle(this.CurrentLevel);
            this.Restart();
        }
        
        public void PreviousLevel()
        {
            this.State = RunState.Start;

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
