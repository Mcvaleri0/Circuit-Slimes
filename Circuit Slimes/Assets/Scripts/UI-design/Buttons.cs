using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Game;
using Puzzle;
using Creator;



namespace UI
{
    public class Buttons : MonoBehaviour
    {
        #region /* Controllers */

        private GameController GameController { get; set; }

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
            this.GameController = GameController.CreateGameController();
        }


        void Start()
        {
            Transform buttonsArea = this.transform.Find("ButtonArea").Find("Buttons");

            this.HintButton = buttonsArea.Find("HorizontalLayout").Find("Hint").GetComponent<Button>();

            this.RemoveButton   = buttonsArea.Find("Remove").GetComponent<Button>();
            this.ResetButton    = buttonsArea.Find("Reset").GetComponent<Button>();
            this.ForwardButton  = buttonsArea.Find("Forward").GetComponent<Button>();
            this.PlayButton     = buttonsArea.Find("Play").GetComponent<Button>();
            this.PauseButton    = buttonsArea.Find("Pause").GetComponent<Button>();
            this.BackwardButton = buttonsArea.Find("Backward").GetComponent<Button>();

            buttonsArea = this.transform.Find("Level Buttons");

            this.PreviousButton = buttonsArea.Find("Previous Level").GetComponent<Button>();
            this.NextButton     = buttonsArea.Find("Next Level").GetComponent<Button>();

            this.HintButton.onClick.AddListener(() => this.GameController.Help());

            this.PreviousButton.onClick.AddListener(() => this.GameController.PreviousLevel());
            this.NextButton.onClick.AddListener(() => this.GameController.NextLevel());
        }

        #endregion
    }
}
