using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Game;



namespace Level
{
    public class CreateLevelForm
    {
        #region /* Level */
        
        private LevelMenu LevelMenu { get; set; }
        
        private LevelController LevelController { get; set; }

        private GameController GameController { get; set; }

        #endregion


        #region /* Form */

        private Transform Form { get; set; }

        #endregion


        #region /* Input Fields */

        private Transform NameInput { get; set; }
        private Transform WidthInput { get; set; }
        private Transform HeightInput { get; set; }

        #endregion


        #region /* Buttons */
        
        private Transform CancelButton { get; set; }
        private Transform CreateButton { get; set; }

        #endregion


        #region /* Data */
        
        private Text Name { get; set; }
        private Text Width { get; set; }
        private Text Height { get; set; }

        #endregion



        #region === Init Mehods ===
        
        public CreateLevelForm(GameController gameController, LevelMenu levelMenu, Transform menu) 
        {
            this.GameController = gameController;

            this.LevelController = this.GameController.LevelController;

            this.LevelMenu = levelMenu;

            this.Form = menu.Find("Form");

            this.NameInput   = this.Form.Find("NameInput");
            this.WidthInput  = this.Form.Find("WidthInput");
            this.HeightInput = this.Form.Find("HeightInput");

            Transform buttons = this.Form.Find("Buttons");
            this.CancelButton = buttons.Find("CancelButton");
            this.CreateButton = buttons.Find("CreateButton");

            this.Name   = this.NameInput.Find("Text").GetComponent<Text>();
            this.Width  = this.WidthInput.Find("Text").GetComponent<Text>();
            this.Height = this.HeightInput.Find("Text").GetComponent<Text>();

            this.DefineCallBackFunction();
        }

        #endregion


        #region === Visibility Methods ===
        
        public void Hide()
        {
            this.Form.gameObject.SetActive(false);
        }


        public void Show()
        {
            this.GoodInput();
            this.Form.gameObject.SetActive(true);
        }

        #endregion


        #region === Buttons Methods ===

        private void DefineCallBackFunction()
        {
            this.CancelButton.GetComponentInChildren<Button>().onClick.AddListener(() => this.LevelMenu.Show(GameController.CREATOR));
            this.CreateButton.GetComponentInChildren<Button>().onClick.AddListener(() => this.CreateLevel());
        }

        #endregion


        #region === Level Methods ===
        
        private void CreateLevel()
        {
            if (this.LevelController.CreateLevel(this.Name.text))
            {
                this.LevelMenu.Update();
                this.GameController.LoadScene(GameController.CREATOR);
            }
            else
            {
                this.WrongInput();
            }
        }

        #endregion


        #region === Input Methods ===

        private void WrongInput()
        {
            this.NameInput.GetComponent<Image>().color = Color.red;
            this.Name.color = Color.white;

            this.WidthInput.GetComponent<Image>().color = Color.red;
            this.Width.color = Color.white;

            this.HeightInput.GetComponent<Image>().color = Color.red;
            this.Height.color = Color.white;
        }


        private void GoodInput()
        {
            this.NameInput.GetComponent<Image>().color = Color.white;
            this.NameInput.GetComponent<InputField>().text = "";
            this.Name.color = Color.black;

            this.WidthInput.GetComponent<Image>().color = Color.white;
            this.WidthInput.GetComponent<InputField>().text = "";
            this.Width.color = Color.black;

            this.HeightInput.GetComponent<Image>().color = Color.white;
            this.HeightInput.GetComponent<InputField>().text = "";
            this.Height.color = Color.black;
        }

        #endregion
    }
}
