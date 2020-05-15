using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Game;
using Puzzle;
using Creator;



namespace UI
{
    public class ButtonController : MonoBehaviour
    {
        #region /* Controllers */

        private GameController Controller { get; set; }

        #endregion


        #region /* Buttons */

        private Button HintButton { get; set; }

        private Button RemoveButton   { get; set; }
        private Button ResetButton    { get; set; }
        private Button ForwardButton   { get; set; }
        private Button PlayButton     { get; set; }
        private Button PauseButton    { get; set; }
        private Button BackwardButton { get; set; }

        private Button PreviousButton { get; set; }
        private Button NextButton     { get; set; }

        #endregion



        #region === Unity Events ===

        private void Awake()
        {
            this.Controller = GameController.CreateGameController();
        }

        #endregion


        #region === Init Methods ===

        public void Initialize()
        {
            this.GetButtons();
            this.InitializeCallBackFunctions();
        }


        public void ReInitialize()
        {
            this.PlayButton.gameObject.SetActive(true);
            this.PauseButton.gameObject.SetActive(false);
        }


        private void InitializeCallBackFunctions()
        {
            this.HintButton.onClick.AddListener(() => this.Controller.Help());

            this.RemoveButton.onClick.AddListener(() => this.Controller.RemoveItemsPlaced());
            this.ResetButton.onClick.AddListener(() => this.Controller.Restart());
            this.ForwardButton.onClick.AddListener(() => this.Controller.Forward());
            this.PlayButton.onClick.AddListener(() => this.Controller.Play());
            this.PauseButton.onClick.AddListener(() => this.Controller.Pause());
            this.BackwardButton.onClick.AddListener(() => this.Controller.Backward());

            this.PreviousButton.onClick.AddListener(() => this.Controller.PreviousLevel());
            this.NextButton.onClick.AddListener(() => this.Controller.NextLevel());
        }


        private void GetButtons()
        {
            Transform buttonsArea = this.transform.Find("ButtonArea").Find("Buttons");

            this.HintButton = buttonsArea.Find("HorizontalLayout").Find("Hint").GetComponent<Button>();

            this.RemoveButton = buttonsArea.Find("Remove").GetComponent<Button>();
            this.ResetButton = buttonsArea.Find("Reset").GetComponent<Button>();
            this.ForwardButton = buttonsArea.Find("Forward").GetComponent<Button>();
            this.PlayButton = buttonsArea.Find("Play").GetComponent<Button>();
            this.PauseButton = buttonsArea.Find("Pause").GetComponent<Button>();
            this.BackwardButton = buttonsArea.Find("Backward").GetComponent<Button>();

            buttonsArea = this.transform.Find("Level Buttons");

            this.PreviousButton = buttonsArea.Find("Previous Level").GetComponent<Button>();
            this.NextButton = buttonsArea.Find("Next Level").GetComponent<Button>();
        }

        #endregion
    }
}
