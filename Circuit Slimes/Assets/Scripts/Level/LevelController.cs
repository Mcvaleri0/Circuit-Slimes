using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Puzzle.Data;



namespace Level
{
    public class LevelController
    {
        #region /* Level Names */

        private const string LEVELS_PATH = "Levels";
        private const string PLAYERS_LEVEL_NAME = "LevelPlayer";
        private const string EMPTY_LEVEL_NAME = "newLevel";

        #endregion


        #region /* Level IDs */

        private const int PLAYERS_LEVEL = -1;
        private const int EMPTY_LEVEL = -2;

        public int CurrentLevel { get; private set; }
        private int nLevels { get; set; }

        #endregion



        #region === Init Methods ===

        public LevelController()
        {
            this.CurrentLevel = 0;
            this.nLevels = 2;
        }


        public Puzzle.Puzzle Initialize()
        {
            return this.LoadLevel(this.CurrentLevel);
        }

        #endregion
        

        #region === Load / Save Level ===

        private Puzzle.Puzzle LoadLevel(int level)
        {
            string name = "Level" + level;

            return PuzzleData.Load(name);
        }


        public void SaveLevel(int level, Puzzle.Puzzle puzzle)
        {
            string name = "Level" + level;

            PuzzleData puzzleData = new PuzzleData(puzzle);
            puzzleData.Save(name);

            Debug.Log("Puzzle Saved. Wait for the file to update");
        }

        #endregion


        #region === Change Level ===

        public Puzzle.Puzzle NextLevel()
        {
            this.CurrentLevel = (this.CurrentLevel + 1) % this.nLevels;

            return this.LoadLevel(this.CurrentLevel);
        }


        public Puzzle.Puzzle PreviousLevel()
        {
            this.CurrentLevel = (this.CurrentLevel - 1) % this.nLevels;

            if (this.CurrentLevel < 0)
            {
                this.CurrentLevel += this.nLevels;
            }

            return this.LoadLevel(this.CurrentLevel);
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
