using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Puzzle;



namespace Creator
{
    public class ScrollMenu
    {
        #region /* Constants */

        private const string BUTTON_PATH = "Prefabs/Creator/Button";

        #endregion

        #region /* UI Elements */

        private Transform Menu { get; set; }
        private Transform MenuContent { get; set; }

        #endregion

        #region /* Creator Attributes */

        private CreatorController Crontroller { get; set; }

        #endregion 


        public ScrollMenu(CreatorController controller, Transform menu, Transform content, List<string> options)
        {
            this.Crontroller = controller;

            this.Menu = menu;
            this.MenuContent = content;

            this.Initialize(options);
        }


        #region === Initialization Methods === 

        private void Initialize(List<string> options)
        {
            this.ResizeMenu();
            this.PopulateContent(options);
        }

        private void ResizeMenu()
        {
            RectTransform rect = this.Menu.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(Screen.width, Screen.height / 4);
            rect.anchoredPosition = new Vector2(0, -3 * Screen.height / 8);
        }

        private void PopulateContent(List<string> options)
        {
            Object button  = Resources.Load(BUTTON_PATH);

            foreach (string opt in options)
            {
                this.InstantiateOption(opt, button);
            }

        }

        #endregion

        #region === Instantiate Methods ===

        private void InstantiateOption(string text, Object button)
        {
            GameObject newObj = (GameObject) GameObject.Instantiate(button, this.MenuContent);
            newObj.GetComponentInChildren<Text>().text = text;

            newObj.GetComponent<Button>().onClick.AddListener(delegate { this.Crontroller.AddBoardItem(text); });
        }

        #endregion

    }
}