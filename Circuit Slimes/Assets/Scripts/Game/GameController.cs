using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Puzzle;
using Creator;
using Hint;
using UI;
using Level;



namespace Game
{
    public class GameController : MonoBehaviour
    {
        #region /* Game Attributes */

        private const string PREFAB_PATH = "Prefabs/GameController";
        private const string CONTROLLER_NAME = "GameController";

        #endregion


        #region /* Scenes Attributes */

        public const string MAIN_MENU = "MainMenu";
        public const string CREATOR = "Creator";
        public const string LEVELS = "Levels";

        #endregion


        #region /* Level Attributes */

        private LevelController LevelController { get; set; }

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
            if (SceneManager.GetActiveScene().name != MAIN_MENU)
            {
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
            this.InitializeLevel();

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


        #region === Level Methods ===

        private void InitializeLevel()
        {
            this.LevelController = new LevelController();

            if (this.Puzzle != null)
            {
                this.Puzzle.Destroy();
            }

            this.Puzzle = this.LevelController.Initialize();
        }


        public int CurrentLevel()
        {
            return this.LevelController.CurrentLevel;
        }


        public void SaveLevel(int level)
        {
            this.LevelController.SaveLevel(level, this.Puzzle);
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
