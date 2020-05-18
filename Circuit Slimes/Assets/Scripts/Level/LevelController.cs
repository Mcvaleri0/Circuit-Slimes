using System.Collections;
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

        public string CurrentLevel { get; private set; }
        private int CurrentInd { get; set; }
        private int nLevels { get; set; }

        #endregion


        #region /* Level List */

        private List<string> Levels { get; set; }

        #endregion


        #region /* Menu Attributes */
        
        private Transform Menu { get; set; }

        #endregion


        #region /* Options Attributes */

        private Transform ScrollMenu { get; set; }
        private Transform Content { get; set; }

        private Object OptionButton { get; set; }

        #endregion


        #region /* Buttons Attributes */

        private Transform BackButton { get; set; }
        
        private Transform NewButton { get; set; }

        #endregion


        #region /* Form Attributes */

        private Transform Form { get; set; }

        private Text LevelName { get; set; }

        #endregion




        #region === Init Methods ===

        public LevelController(GameController Controller, Transform transform)
        {
            this.Controller = Controller;
            this.InitializeMenu(transform);
            this.InitializeButtons();
            this.InitializeOptions();
            this.InitializeForm();
        }

        #endregion


        #region === Menu Methods ===

        private void InitializeMenu(Transform transform)
        {
            this.Menu = transform.Find("Canvas").Find("Menu");
            this.HideLevelMenu();
        }


        public void ShowLevelMenu(string nextScene)
        {
            this.PopulateMenu();
            this.DefineOptionsCallBack(nextScene);
            this.ShowButtons(nextScene);
            this.Menu.gameObject.SetActive(true);
        }


        public void HideLevelMenu()
        {
            this.Menu.gameObject.SetActive(false);
        }


        private void PopulateMenu()
        {
            if (this.Levels == null)
            {
                this.Levels = FileHelper.GetFileList(FileHelper.LEVELS_PATH).Where(f => !f.Equals(FileHelper.EMPTY_LEVEL)).ToList();

                foreach (string level in this.Levels)
                {
                    GameObject newObj = (GameObject) GameObject.Instantiate(this.OptionButton, this.Content);
                    newObj.GetComponentInChildren<Text>().text = level;
                }
            }
        }


        private void ClearMenu()
        {
            foreach (Transform option in this.Content)
            {
                GameObject.Destroy(option.gameObject);
            }

            this.Levels = null;
        }

        #endregion


        #region === Options Methods ===

        private void InitializeOptions()
        {
            this.ScrollMenu = this.Menu.Find("LevelsMenu");
            this.Content = this.ScrollMenu.Find("Viewport").Find("Content");

            this.PopulateMenu();

            this.CurrentInd = 0;
            this.CurrentLevel = this.Levels[this.CurrentInd];
            this.nLevels = this.Levels.Count;
        }


        private void DefineOptionsCallBack(string nextScene)
        {
            foreach (Transform option in this.Content)
            {
                Button optionButton = option.GetComponentInChildren<Button>();
                string level = option.GetComponentInChildren<Text>().text;

                optionButton.onClick.RemoveAllListeners();
                optionButton.onClick.AddListener(() => this.ChooseLevel(level, nextScene));
            }
        }

        #endregion


        #region === Buttons Methods ===

        private void InitializeButtons()
        {
            this.BackButton = this.Menu.Find("BackButton");
            this.BackButton.GetComponent<Button>().onClick.AddListener(() => this.Controller.ShowMainMenu());

            this.OptionButton = Resources.Load(FileHelper.BUTTON_PATH);

            this.NewButton = this.Menu.Find("NewButton");

            this.NewButton.GetComponentInChildren<Button>().onClick.AddListener(() => this.ShowForm());
        }


        private void ShowButtons(string scene)
        {
            if (scene.Equals(GameController.LEVELS))
            {
                this.NewButton.gameObject.SetActive(false);
            }
            else
            {
                #if UNITY_EDITOR
                    this.NewButton.gameObject.SetActive(true);
                #else
                    this.NewButton.gameObject.SetActive(false);
                #endif
            }
        }

        #endregion


        #region === Form Methods ===

        private void InitializeForm()
        {
            this.Form = this.Menu.Find("NameForm");
            this.LevelName = this.Form.Find("InputField").Find("Text").GetComponent<Text>();

            this.Form.Find("CancelButton").GetComponentInChildren<Button>().onClick.AddListener(() => this.HideForm());
            this.Form.Find("CreateButton").GetComponentInChildren<Button>().onClick.AddListener(() => this.CreateLevel());

            this.Form.gameObject.SetActive(false);
        }


        private void ShowForm()
        {
            this.ScrollMenu.gameObject.SetActive(false);
            this.NewButton.gameObject.SetActive(false);
            this.Form.gameObject.SetActive(true);
        }


        private void HideForm()
        {
            this.Form.gameObject.SetActive(false);
            this.NewButton.gameObject.SetActive(true);
            this.ScrollMenu.gameObject.SetActive(true);
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


        #region === New Level Methods ===

        private void CreateLevel()
        {
            if (!this.GoodLevelName(this.LevelName.text))
            {
                this.Form.Find("InputField").GetComponent<Image>().color = Color.red;
                this.LevelName.color = Color.white;
            }
            else
            {
                this.Form.Find("InputField").GetComponent<Image>().color = Color.white;
                this.LevelName.color = Color.black;

                string empty = FileHelper.LoadLevel(FileHelper.EMPTY_LEVEL);
                FileHelper.WriteLevel(empty, this.LevelName.text);

                this.CurrentLevel = this.LevelName.text;

                this.HideForm();
                
                this.ClearMenu();

                this.Controller.LoadScene(GameController.CREATOR);
            }
        }


        private bool GoodLevelName(string name)
        {
            return ((name.Length > 0) && (!name.Equals(FileHelper.EMPTY_LEVEL)) && (!this.Levels.Contains(name)));
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
