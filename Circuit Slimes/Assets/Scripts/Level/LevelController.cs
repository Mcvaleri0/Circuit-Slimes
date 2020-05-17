﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using Game;
using Puzzle.Data;



namespace Level
{
    public class LevelController
    {
        #region /* Game Controller */

        private GameController Controller { get; set; }

        #endregion


        #region /* Level IDs */

        private const string EMPTY_LEVEL = "EmptyLevel";

        public string CurrentLevel { get; private set; }
        private int CurrentInd { get; set; }
        private int nLevels { get; set; }

        #endregion


        #region /* Level List */

        private List<string> Levels { get; set; }

        #endregion


        #region /* UI Attributes */
        
        private Transform MenuTransform { get; set; }
        private Transform ContentTransform { get; set; }
        private Transform BackButton { get; set; }

        private Object OptionButton { get; set; }

        #endregion



        #region === Init Methods ===

        public LevelController(GameController Controller, Transform transform)
        {
            this.Controller = Controller;
            this.InitializeMenu(transform);
        }


        private void InitializeMenu(Transform transform)
        {
            this.MenuTransform = transform.Find("Canvas").Find("Menu");
            this.HideLevelMenu();

            this.ContentTransform = this.MenuTransform.Find("LevelsMenu").Find("Viewport").Find("Content");

            this.BackButton = this.MenuTransform.Find("BackButton");
            this.BackButton.GetComponent<Button>().onClick.AddListener(() => this.Controller.ShowMainMenu());

            this.OptionButton = Resources.Load(FileHelper.BUTTON_PATH);

            this.Levels = FileHelper.GetFileList(FileHelper.LEVELS_PATH).Where(f => !f.Equals(EMPTY_LEVEL)).ToList();
            this.PopulateMenu();

            this.CurrentInd = 0;
            this.CurrentLevel = this.Levels[this.CurrentInd];
            this.nLevels = this.Levels.Count;
        }

        #endregion


        #region === Menu Methods ===

        public void ShowLevelMenu(string nextScene)
        {
            this.DefineOptionsCallBack(nextScene);
            this.MenuTransform.gameObject.SetActive(true);
        }


        public void HideLevelMenu()
        {
            this.MenuTransform.gameObject.SetActive(false);
        }


        private void PopulateMenu()
        {
            foreach (string level in this.Levels)
            {
                GameObject newObj = (GameObject) GameObject.Instantiate(this.OptionButton, this.ContentTransform);
                newObj.GetComponentInChildren<Text>().text = level;
            }
        }


        private void DefineOptionsCallBack(string nextScene)
        {
            foreach (Transform option in this.ContentTransform)
            {
                Button optionButton = option.GetComponentInChildren<Button>();
                string level = option.GetComponentInChildren<Text>().text;

                optionButton.onClick.RemoveAllListeners();
                optionButton.onClick.AddListener(() => this.ChooseLevel(level, nextScene));
            }
        }

        #endregion


        #region === Load / Save Level ===

        public Puzzle.Puzzle LoadLevel()
        {
            return this.LoadLevel(this.CurrentLevel);
        }


        private Puzzle.Puzzle LoadLevel(string LevelName)
        {
            return PuzzleData.Load(LevelName);
        }


        public void SaveLevel(string level, Puzzle.Puzzle puzzle)
        {
            PuzzleData puzzleData = new PuzzleData(puzzle);
            puzzleData.Save(level);

            Debug.Log("Puzzle Saved. Wait for the file to update");
        }

        #endregion


        #region === Change Level ===

        private void ChooseLevel(string levelName, string nextScene)
        {
            this.CurrentLevel = levelName;
            this.CurrentInd   = this.Levels.IndexOf(this.CurrentLevel);
            this.Controller.LoadScene(nextScene);
        }


        public Puzzle.Puzzle NextLevel()
        {
            this.CurrentInd = (this.CurrentInd + 1) % this.nLevels;

            this.CurrentLevel = this.Levels[this.CurrentInd];

            return this.LoadLevel(this.CurrentLevel);
        }


        public Puzzle.Puzzle PreviousLevel()
        {
            this.CurrentInd = (this.CurrentInd - 1) % this.nLevels;

            if (this.CurrentInd < 0)
            {
                this.CurrentInd += this.nLevels;
            }

            this.CurrentLevel = this.Levels[this.CurrentInd];

            return this.LoadLevel(this.CurrentLevel);
        }

        #endregion


        //#region === Player's Levels Methods ===

        //private bool CreatePlayersLevel()
        //{
        //    string path = Path.Combine(Application.persistentDataPath, LEVELS_PATH);
        //    string completePath = Path.Combine(path, PLAYERS_LEVEL_NAME + ".json");

        //    if (!Directory.Exists(path))
        //    {
        //        Directory.CreateDirectory(path);
        //        var file = File.Create(completePath);
        //        file.Dispose();
        //        return true;
        //    }
        //    else if (!File.Exists(completePath))
        //    {
        //        var file = File.Create(completePath);
        //        file.Dispose();
        //        return true;
        //    }

        //    return false;
        //}

        //#endregion

    }
}
