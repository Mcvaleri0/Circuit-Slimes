using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Puzzle;
using Puzzle.Data;
using Creator;
using Hint;
using UI;



namespace Game
{
    public class GameController : MonoBehaviour
    {
        #region /* Level Attributes */

        private const string LEVELS_PATH = "Levels";
        private const string PLAYERS_LEVEL_NAME = "LevelPlayer";
        private const string EMPTY_LEVEL_NAME = "newLevel";

        public const int PLAYERS_LEVEL = -1;
        public const int EMPTY_LEVEL = -2;

        public int CurrentLevel { get; private set; }

        // set on editor
        public int nLevels;

        #endregion


        #region /* Puzzle Attibutes */

        public Puzzle.Puzzle Puzzle { get; private set; }

        private PuzzleController PuzzleController { get; set; }

        #endregion


        #region /* Creator Attributes */

        private CreatorController CreatorController { get; set; }

        private bool Creator { get; set; }

        #endregion


        #region /* Hint Attributes */

        private HintController Hint { get; set; }

        #endregion


        #region /* Buttons Attributes */

        private ButtonController ButtonController { get; set; }

        #endregion


        #region /* Camera Atributes */

        private CameraController CameraController { get; set; }

        #endregion

        #region /* Scenes Attributes */

        public const string MAIN_MENU = "MainMenu";
        public const string CREATOR   = "Creator";
        public const string LEVELS    = "Levels";

        #endregion


        #region /* Game Attributes */

        private const string PREFAB_PATH = "Prefabs/GameController";
        private const string CONTROLLER_NAME = "GameController";

        #endregion



        #region === Unity Events ===

        void OnEnable()
        {
            SceneManager.sceneLoaded += this.OnSceneLoaded;
        }


        void OnDisable()
        {
            SceneManager.sceneLoaded -= this.OnSceneLoaded;
        }

        #endregion


        #region === Game Methods ===

        public static GameController CreateGameController()
        {
            GameObject controllerObj = GameObject.Find(CONTROLLER_NAME);

            if (controllerObj == null)
            {
                controllerObj = GameObject.Instantiate(Resources.Load(PREFAB_PATH)) as GameObject;
                controllerObj.name = CONTROLLER_NAME;

                GameObject.DontDestroyOnLoad(controllerObj);
            }

            GameController controller = controllerObj.GetComponent<GameController>();

            return controller;
        }


        public void QuitGame()
        {
            #if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }


        #endregion


        #region === Scenes' Methods ===

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            this.InitializeScene();
        }


        private void InitializeScene()
        {
            if (SceneManager.GetActiveScene().name != MAIN_MENU)
            {
                this.LoadLevel(this.CurrentLevel);

                this.InitialiazeControllers();

                this.EditMode();
            }
        }


        public void LoadScene(string name)
        {
            this.CurrentLevel = 0;

            switch (name)
            {
                case MAIN_MENU:
                    break;

                case CREATOR:
                    this.Creator = true;
                    break;

                case LEVELS:
                    this.Creator = false;
                    break;
            }

            SceneManager.LoadSceneAsync(name);
        }

        #endregion


        #region === Controller Methods ===

        private void InitialiazeControllers()
        {
            GameObject controller = GameObject.Find("CreatorController");
            if (controller != null)
            {
                this.CreatorController = controller.GetComponent<CreatorController>();

                if (SceneManager.GetActiveScene().name == CREATOR)
                {
                    this.Creator = true;
                }

                this.CreatorController.Initialize(this.Puzzle, this.Creator);
            }

            controller = GameObject.Find("PuzzleController");
            if (controller != null)
            {
                this.PuzzleController = controller.GetComponent<PuzzleController>();
                this.PuzzleController.Initialize(this.Puzzle);
            }

            controller = GameObject.Find("UI");
            if (controller != null)
            {
                this.ButtonController = controller.GetComponent<ButtonController>();
                this.ButtonController.Initialize();
            }
     
            if (Camera.main != null)
            {
                this.CameraController = Camera.main.GetComponent<CameraController>();
                this.CameraController.Initialize(this.Puzzle);
            }

            this.InitializeHints();
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

            if (this.ButtonController != null)
            {
                this.ButtonController.ReInitialize();
            }

            if (Camera.main != null)
            {
                this.CameraController.Initialize(this.Puzzle);
            }
        }

        #endregion


        #region === Level Functions ===

        private void LoadLevel(int level)
        {
            string path;
            string name;

            if (this.Puzzle != null)
            {
                this.Puzzle.Destroy();
            }

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
            this.CurrentLevel = (this.CurrentLevel + 1) % this.nLevels;

            this.LoadLevel(this.CurrentLevel);

            this.UpdateControllers();
        }


        public void PreviousLevel()
        {
            this.CurrentLevel = (this.CurrentLevel - 1) % this.nLevels;

            if (this.CurrentLevel < 0)
            {
                this.CurrentLevel += this.nLevels;
            }

            this.LoadLevel(this.CurrentLevel);

            this.UpdateControllers();
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


        #region === Hint Methods ===

        private void InitializeHints()
        {
            this.Hint = new HintController();
        }


        public void Help()
        {
            this.Hint.Help();
        }

        #endregion


        #region === Puzzle Methods ===

        public void RemoveItemsPlaced()
        {
            if (this.CreatorController.isActiveAndEnabled)
            {
                this.CreatorController.RemoveItemsPlaced();
            }
        }

        #endregion


        #region === Simuation Methods ===

        public void Play()
        {
            this.PlayMode();
            this.PuzzleController.Play();
        }


        public void Pause()
        {
            this.PuzzleController.Pause();
        }


        public void Restart()
        {
            this.PuzzleController.Restart();
        }


        public void Forward()
        {
            this.PlayMode();
            this.PuzzleController.StepForward();
        }


        public void Backward()
        {
            this.PuzzleController.StepBack();
        }


        public void RewindFinished()
        {
            this.EditMode();
            this.ButtonController.ReInitialize();
        }

        #endregion


        #region === Camera Modes ===

        private void EditMode()
        {
            this.CreatorController.gameObject.SetActive(true);
            this.CameraController.EditMode();
        }


        private void PlayMode()
        {
            this.CameraController.PlayMode();
            this.CreatorController.gameObject.SetActive(false);
        }

        #endregion
    }
}
