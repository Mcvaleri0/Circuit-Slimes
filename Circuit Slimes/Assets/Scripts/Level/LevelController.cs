using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Puzzle.Data;



namespace Level
{
    public class LevelController
    {
        #region /* Level List */
        
        public List<string> Levels { get; private set; }

        private int nLevels { get; set; }

        #endregion


        #region /* Current Level */
        
        private string CurrentLevel { get; set; }

        public int CurrentInd { get; set; }

        #endregion



        #region === Init Methods ===
        
        public LevelController()
        {
            this.GetLevels();

            this.CurrentInd = 0;
            this.CurrentLevel = this.Levels[this.CurrentInd];
        }


        public bool CreateLevel(string name, int width, int height)
        {
            if (this.ValidName(name))
            {
                Puzzle.Puzzle empty = Puzzle.Puzzle.CreateEmpty(width, height);
                empty.gameObject.SetActive(false);
                this.SaveLevel(empty, name);
                this.GetLevels();
                this.Current(name);
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion


        #region === Current Level Methods ===
        
        public void Current(string level)
        {
            this.CurrentLevel = level;
            this.CurrentInd = this.Levels.IndexOf(level);
        }


        public Puzzle.Puzzle LoadLevel()
        {
            return PuzzleData.Load(this.CurrentLevel);
        }


        public void SaveLevel(Puzzle.Puzzle puzzle)
        {
            this.SaveLevel(puzzle, this.CurrentLevel);
        }


        private void SaveLevel(Puzzle.Puzzle puzzle, string name)
        {
            PuzzleData puzzleData = new PuzzleData(puzzle);
            puzzleData.Save(name);

            Debug.Log("Puzzle Saved. Wait for the file to update");
        }

        #endregion


        #region === Next / Previous Level Methods ===

        public Puzzle.Puzzle NextLevel()
        {
            this.CurrentInd = (this.CurrentInd + 1) % this.nLevels;

            this.CurrentLevel = this.Levels[this.CurrentInd];

            return this.LoadLevel();
        }


        public Puzzle.Puzzle PreviousLevel()
        {
            this.CurrentInd = (this.CurrentInd - 1) % this.nLevels;

            if (this.CurrentInd < 0)
            {
                this.CurrentInd += this.nLevels;
            }

            this.CurrentLevel = this.Levels[this.CurrentInd];

            return this.LoadLevel();
        }

        #endregion


        #region === Level Name Methods ===

        private void GetLevels()
        {
            this.Levels = FileHelper.GetFileList(FileHelper.LEVELS_PATH);
           
            this.nLevels = this.Levels.Count;
        }


        private bool ValidName(string name)
        {
            return ((name.Length > 0) && (!this.Levels.Contains(name)));
        }

        #endregion
    }
}
