using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Game;



namespace Level
{
    public class LevelMenu
    {
        #region /* Game Controller */
        
        private GameController Controller { get; set; }

        #endregion


        #region /* Menu */

        private Transform Menu { get; set; }

        #endregion


        #region /* Buttons */

        private Transform BackButton { get; set; }
        private Transform NewButton { get; set; }

        #endregion


        #region /* Scroll */
        
        private Scroll Scroll { get; set; }

        #endregion


        #region /* Form */
        
        private CreateLevelForm Form { get; set; }

        #endregion



        #region === Init Methods ===

        public LevelMenu(GameController controller)
        {
            this.Controller = controller;

            this.Menu = this.Controller.transform.Find("Canvas").Find("Menu");

            this.BackButton = this.Menu.Find("BackButton");
            this.NewButton  = this.Menu.Find("NewButton");

            this.Scroll = new Scroll(controller, this.Menu);

            this.Form = new CreateLevelForm(this.Controller, this, this.Menu);
        }


        public void Initialize(List<string> levels)
        {
            this.DefineCallBackFunction();
            this.Scroll.Populate(levels);
        }

        #endregion


        #region === Menu Methods ===

        public void Hide()
        {
            this.HideButtons();
            this.Scroll.Hide();
            this.Form.Hide();
        }


        public void Show(string nextScene)
        {
            this.Form.Hide();
            this.ShowButtons(nextScene);
            this.Scroll.Show(nextScene);
        }


        public void Update()
        {
            this.Scroll.Update(this.Controller.Levels());
        }

        #endregion


        #region === Buttons Methods ===

        private void DefineCallBackFunction()
        {
            this.BackButton.GetComponentInChildren<Button>().onClick.AddListener(() => this.Controller.GoToMainMenu());
            this.NewButton.GetComponentInChildren<Button>().onClick.AddListener(() => this.ShowForm());
        }


        private void HideButtons()
        {
            this.BackButton.gameObject.SetActive(false);
            this.NewButton.gameObject.SetActive(false);
        }


        private void ShowButtons(string nextScene)
        {
            this.BackButton.gameObject.SetActive(true);

            #if UNITY_EDITOR
                if (nextScene.Equals(GameController.CREATOR))
                {
                    this.NewButton.gameObject.SetActive(true);
                }
                else
                {
                    this.NewButton.gameObject.SetActive(false);
                }
            #else
                this.NewButton.gameObject.SetActive(false);
            #endif
        }

        #endregion


        #region === Form Methods ===
        
        private void ShowForm()
        {
            this.NewButton.gameObject.SetActive(false);
            this.Scroll.Hide();
            this.Form.Show();
        }
        
        #endregion
    }
}
