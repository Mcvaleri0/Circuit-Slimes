using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Game;



namespace Level
{
    public class Scroll
    {
        #region /* Game Controller */
        
        private GameController Controller { get; set; }
        
        #endregion

        #region /* UI Elements */

        private Transform ViewPoint { get; set; }
        private Transform Content { get; set; }

        private Object OptionButton { get; set; }

        #endregion



        #region === Init Methods ===
        
        public Scroll(GameController controller, Transform menu)
        {
            this.Controller = controller;

            this.ViewPoint = menu.Find("LevelsMenu");
            this.Content = this.ViewPoint.Find("Viewport").Find("Content");

            this.OptionButton = Resources.Load(FileHelper.BUTTON_PATH);
        }

        #endregion


        #region === Visibility Methods ===
        
        public void Hide()
        {
            this.ViewPoint.gameObject.SetActive(false);
        }


        public void Show(string nextScene)
        {
            this.ViewPoint.gameObject.SetActive(true);
            this.DefineCallBackFunction(nextScene);
        }

        #endregion


        #region === Content Methods ===
        
        public void Populate(List<string> options)
        {
            foreach (string level in options)
            {
                GameObject newObj = (GameObject) GameObject.Instantiate(this.OptionButton, this.Content);
                newObj.GetComponentInChildren<Text>().text = level;
            }
        }


        private void Clear()
        {
            foreach (Transform option in this.Content)
            {
                GameObject.Destroy(option.gameObject);
            }
        }


        public void Update(List<string> newOptions)
        {
            this.Clear();
            this.Populate(newOptions);
        }


        private void DefineCallBackFunction(string nextScene)
        {
            foreach (Transform option in this.Content)
            {
                Button optionButton = option.GetComponentInChildren<Button>();
                string level = option.GetComponentInChildren<Text>().text;

                optionButton.onClick.RemoveAllListeners();
                optionButton.onClick.AddListener(() => this.Controller.ChooseLevel(level, nextScene));
            }
        }

        #endregion
    }
}
