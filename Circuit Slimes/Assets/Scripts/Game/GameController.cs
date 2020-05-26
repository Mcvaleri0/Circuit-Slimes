using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
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
        private MainMenuCameraController MenuCamera { get; set; }

        #endregion


        #region /* Level Menu Attributes */
        
        private LevelMenu LevelMenu { get; set; }

        private bool StartGame = false;

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


        #region /* Audio Atributes */

        private AudioManager AudioManager { get; set; }
        private const string AUDIO_PREFAB_PATH = "Prefabs/Game/AudioManager";
        private const string AUDIO_NAME = "AudioManager";

        #endregion


        #region /* Hint Attributes */

        private HintController Hint { get; set; }

        #endregion


        #region /* Analytics Attributes */
        
        public AnalyticsController AnalyticsController { get; private set; }
        private const string ANALYTICS_PREFAB_PATH = "Prefabs/Game/AnalyticsController";
        private const string ANALYTICS_NAME = "AnalyticsController";

        #endregion



        #region === Unity Events ===

        private void Start()
        {
            //Application.targetFrameRate = 60;

            if (SceneManager.GetActiveScene().name == MAIN_MENU)
            {
                this.MenuCamera.GoToSplashScreen();
                this.HideMainMenu();
            }
        }

        private void Update()
        {
            if (SceneManager.GetActiveScene().name == MAIN_MENU)
            {
                if (!this.StartGame && Input.anyKey)
                {
                    this.StartGame = true;
                    this.MenuCamera.GoToMenu();
                    this.ShowMainMenu();
                }
            }
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

                var gameController = controllerObj.GetComponent<GameController>();

                //Audio Controller
                gameController.InitializeAudioManager();

                //hint controller
                gameController.InitializeHints();
                
                // analytics
                controllerScrp.InitializeAnalytics();
            }

            GameController controller = controllerObj.GetComponent<GameController>();

            return controller;
        }


        public void QuitGame()
        {
            this.AnalyticsController.GameOver();

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

                this.AudioManager.PlayMusic("MenuBackgroundMusic");
            }
            else
            {
                this.LevelController.HintController = this.Hint;

                this.Puzzle = this.LevelController.LoadLevel();
                this.InitialiazeControllers();

                this.AudioManager.PlayMusic("LevelBackgroundMusic");
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

            if(this.MenuCamera == null)
            {
                this.MenuCamera = GameObject.Find("MenuCamera").GetComponent<MainMenuCameraController>();
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

            this.MenuCamera.GoToMenu();
        }


        public void BackButton()
        {
            if (SceneManager.GetActiveScene().name == LEVELS)
            {
                //this.AnalyticsController.LevelQuit();
            }

            this.LoadScene(GameController.MAIN_MENU);
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

            if(nextScene == GameController.LEVELS)
            {
                this.MenuCamera.GoToPlay();
            }
            else if(nextScene == GameController.CREATOR)
            {
                this.MenuCamera.GoToEdit();
            }
        }

        #endregion


        #region === Level Methods ===

        private void InitializeLevel()
        {
            if (this.LevelController == null)
            {
                this.LevelController = new LevelController(this.Hint);

                this.Hint.LevelController = this.LevelController;
            }
        }


        public List<string> Levels()
        {
            return this.LevelController.Levels;
        }


        public void ChooseLevel(string level, string nextScene)
        {
            if (nextScene == LEVELS)
            {
                this.AnalyticsController.LevelStart(level);
            }

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

            //AnalyticsEvent.LevelSkip(this.LevelController.CurrentInd);
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


        #region === Audio Manager

        public void InitializeAudioManager()
        {
            //spawn AudioManager as child of game controller
            GameObject AudioManagerObj = GameObject.Instantiate(Resources.Load(AUDIO_PREFAB_PATH)) as GameObject;
            AudioManagerObj.transform.parent = transform;
            AudioManagerObj.name = AUDIO_NAME;
            this.AudioManager = AudioManagerObj.GetComponent<AudioManager>();

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


        #region === Analytics Methods ===
        
        public void InitializeAnalytics()
        {
            //spawn AudioManager as child of game controller
            GameObject AnalyticsObj = GameObject.Instantiate(Resources.Load(ANALYTICS_PREFAB_PATH)) as GameObject;
            AnalyticsObj.transform.parent = transform;
            AnalyticsObj.name = ANALYTICS_NAME;
            this.AnalyticsController = AnalyticsObj.GetComponent<AnalyticsController>();

            this.AnalyticsController.GameStart();
        }

        #endregion
    }
}
