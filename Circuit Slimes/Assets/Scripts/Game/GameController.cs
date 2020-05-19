using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using UI;
using Hint;
using Level;
using Puzzle;
using Creator;



namespace Game
{
    public class GameController : MonoBehaviour
    {
        #region /* Game Attributes */

        private const string PREFAB_PATH = "Prefabs/Game/GameController";
        private const string CONTROLLER_NAME = "GameController";

        #endregion


        #region /* Scenes Attributes */

        public const string MAIN_MENU = "MainMenu";
        public const string CREATOR = "Creator";
        public const string LEVELS = "Levels";

        #endregion


        #region /* Main Menu Attributes */

        private GameObject MainMenu { get; set; }
        private GameObject QuitButton { get; set; }

        #endregion


        #region /* Level Menu Attributes */
        
        private LevelMenu LevelMenu { get; set; }

        #endregion


        #region /* Level Attributes */

        public LevelController LevelController { get; private set; }

        #endregion


        #region /* Creator Attributes */

        private CreatorController CreatorController { get; set; }

        private bool Creator { get; set; }

        #endregion


        #region /* Puzzle Attibutes */

        public Puzzle.Puzzle Puzzle { get; private set; }

        private PuzzleController PuzzleController { get; set; }

        #endregion


        #region /* UI Attributes */

        private ButtonController ButtonController { get; set; }

        #endregion


        #region /* Camera Atributes */

        private CameraController CameraController { get; set; }

        #endregion


        #region /* Hint Attributes */

        private HintController Hint { get; set; }

        #endregion



        #region === Unity Events ===

        private void Start()
        {
            Application.targetFrameRate = 60;
        }


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
            this.InitializeLevel();
            this.InitializeLevelMenu();

            if (SceneManager.GetActiveScene().name == MAIN_MENU)
            {
                this.InitializeMainMenu();
            }
            else
            {
                this.Puzzle = this.LevelController.LoadLevel();
                this.InitialiazeControllers();
            }
        }


        public void LoadScene(string name)
        {
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


        #region === Controllers Methods ===

        private void InitialiazeControllers()
        {
            this.InitializeCreator();

            this.InitiliazePuzzle();

            this.InitializeUI();

            this.InitializeCamera();

            this.InitializeHints();
        }


        private void UpdateControllers()
        {
            this.UpdateCreator();

            this.UpdatePuzzle();

            this.UpdateUI();

            this.UpdateCamera();
        }

        #endregion


        #region === Main Menu Methods ===

        private void InitializeMainMenu()
        {
            if (this.MainMenu == null)
            {
                this.MainMenu = GameObject.Find("MainMenu");
            }

            if (this.QuitButton == null)
            {
                this.QuitButton = GameObject.Find("QuitButton");
            }
        }


        private void HideMainMenu()
        {
            this.MainMenu.SetActive(false);
            this.QuitButton.SetActive(false);
        }


        public void ShowMainMenu()
        {
            this.MainMenu.SetActive(true);
            this.QuitButton.SetActive(true);
        }


        public void GoToMainMenu()
        {
            this.LevelMenu.Hide();
            this.ShowMainMenu();
        }

        #endregion


        #region === Level Menu Methods ===
        
        private void InitializeLevelMenu()
        {
            if (this.LevelMenu == null)
            {
                this.LevelMenu = new LevelMenu(this);
                this.LevelMenu.Initialize(this.LevelController.Levels);
            }

            this.LevelMenu.Hide();
        }


        public void ShowLevelMenu(string nextScene)
        {
            this.HideMainMenu();
            this.LevelMenu.Show(nextScene);
        }

        #endregion


        #region === Level Methods ===

        private void InitializeLevel()
        {
            if (this.LevelController == null)
            {
                this.LevelController = new LevelController();
            }
        }


        public List<string> Levels()
        {
            return this.LevelController.Levels;
        }


        public void ChooseLevel(string level, string nextScene)
        {
            this.LevelMenu.Hide();
            this.LevelController.Current(level);
            this.LoadScene(nextScene);
        }


        public void NextLevel()
        {
            if (this.Puzzle != null)
            {
                this.Puzzle.Destroy();
            }

            this.Puzzle = this.LevelController.NextLevel();

            this.UpdateControllers();
        }


        public void PreviousLevel()
        {
            if (this.Puzzle != null)
            {
                this.Puzzle.Destroy();
            }

            this.Puzzle = this.LevelController.PreviousLevel();

            this.UpdateControllers();
        }


        public void SaveLevel()
        {
            this.LevelController.SaveLevel(this.Puzzle);
        }

        #endregion


        #region === Creator Methods ===

        private void InitializeCreator()
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
        }


        private void UpdateCreator()
        {
            if (this.CreatorController != null)
            {
                this.CreatorController.UpdateInfo(this.Puzzle);
            }
        }

        #endregion


        #region === Puzzle Methods ===

        private void InitiliazePuzzle()
        {
            GameObject controller = GameObject.Find("PuzzleController");
            if (controller != null)
            {
                this.PuzzleController = controller.GetComponent<PuzzleController>();
                this.PuzzleController.Initialize(this.Puzzle);
            }
        }


        private void UpdatePuzzle()
        {
            if (this.PuzzleController != null)
            {
                this.PuzzleController.UpdatePuzzle(this.Puzzle);
            }
        }


        public void RemoveItemsPlaced()
        {
            if (this.CreatorController.isActiveAndEnabled)
            {
                this.CreatorController.RemoveItemsPlaced();
            }
        }

        #endregion


        #region === UI Methods ===

        private void InitializeUI()
        {
            GameObject controller = GameObject.Find("UI");
            if (controller != null)
            {
                this.ButtonController = controller.GetComponent<ButtonController>();
                this.ButtonController.Initialize();
            }
        }


        private void UpdateUI()
        {
            if (this.ButtonController != null)
            {
                this.ButtonController.ReInitialize();
            }
        }

        #endregion


        #region === Camera Methods ===

        private void InitializeCamera()
        {
            if (Camera.main != null)
            {
                this.CameraController = Camera.main.GetComponent<CameraController>();
                this.CameraController.Initialize(this.Puzzle);
            }

            this.EditMode();
        }


        private void UpdateCamera()
        {
            if (Camera.main != null)
            {
                this.CameraController.Initialize(this.Puzzle);
            }

            this.EditMode();
        }


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

    }
}
