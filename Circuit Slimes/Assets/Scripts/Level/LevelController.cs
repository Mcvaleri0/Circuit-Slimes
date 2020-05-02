using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using Puzzle;
using Puzzle.Data;
using Creator;



namespace Level
{
    public class LevelController : MonoBehaviour
    {
        #region /* Level Attributes */

        private const string LEVELS_PATH = "Levels";
        private const string PLAYERS_LEVEL_NAME = "LevelPlayer";
        private const string EMPTY_LEVEL_NAME = "newLevel";

        public const int PLAYERS_LEVEL = -1;
        public const int EMPTY_LEVEL = -2;

        // set on editor
        public int CurrentLevel;
        public int nLevels;

        #endregion


        #region /* Puzzle Attibutes */

        public Puzzle.Puzzle Puzzle { get; private set; }

        private PuzzleController PuzzleController { get; set; }

        #endregion


        #region /* Creator Attributes */

        private CreatorController CreatorController { get; set; }

        // set on editor
        public bool Creator;

        #endregion



        #region === Unity Events ===

        private void Awake()
        {
            GameObject.DontDestroyOnLoad(this);            
        }


        // Start is called before the first frame update
        void Start()
        {
            this.LoadLevel(this.CurrentLevel);

            this.InitialiazeControllers();
        }

        #endregion


        #region === Controller Methods ===

        private void InitialiazeControllers()
        {
            GameObject controller = GameObject.Find("CreatorController");
            if (controller != null)
            {
                this.CreatorController = controller.GetComponent<CreatorController>();
                this.CreatorController.Initialize(this, this.Puzzle, this.Creator);
            }

            controller = GameObject.Find("PuzzleController");
            if (controller != null)
            {
                this.PuzzleController = controller.GetComponent<PuzzleController>();
                this.PuzzleController.Initialize(this.Puzzle);
            }
        }


        private void UpdateControllers()
        {
            if (this.CreatorController != null)
            {
                this.CreatorController.UpdateInfo(this.Puzzle);
            }

            if (this.PuzzleController != null)
            {
                this.PuzzleController.UpdatePuzzle(this.Puzzle);
            }
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

                /*
                if (!System.IO.File.Exists(Path.Combine(path, name)))
                {
                    name = EMPTY_LEVEL_NAME;
                    Debug.Log("File not found - Loading New Level");
                }
                */
            }

            this.Puzzle = PuzzleData.Load(path, name);

            //this.WinCondition = this.Puzzle.WinCondition;
        }


        public void SaveLevel(int level)
        {
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

        public void NextLevel()
        {
            //this.State = RunState.Idle;

            this.CurrentLevel = (this.CurrentLevel + 1) % this.nLevels;

            this.LoadLevel(this.CurrentLevel);

            this.UpdateControllers();

            //this.Restart();
        }


        public void PreviousLevel()
        {
            //this.State = RunState.Idle;

            this.CurrentLevel = (this.CurrentLevel - 1) % this.nLevels;

            if (this.CurrentLevel < 0)
            {
                this.CurrentLevel += this.nLevels;
            }

            this.LoadLevel(this.CurrentLevel);

            this.UpdateControllers();

            //this.Restart();
        }

        #endregion


        #region === Player's Levels Methods ===

        private bool CreatePlayersLevel()
        {
            string path = Path.Combine(Application.persistentDataPath, LEVELS_PATH);
            string completePath = Path.Combine(path, PLAYERS_LEVEL_NAME + ".json");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                var file = File.Create(completePath);
                file.Dispose();
                return true;
            }
            else if (!File.Exists(completePath))
            {
                var file = File.Create(completePath);
                file.Dispose();
                return true;
            }

            return false;
        }

        #endregion

    }
}
