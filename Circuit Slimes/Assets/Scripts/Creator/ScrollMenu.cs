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

        private const string BUTTONS_PATH = "Prefabs/Creator/";

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
            Object optionButton  = Resources.Load(BUTTONS_PATH + "OptionButton");

            foreach (string opt in options)
            {
                this.InstantiateOption(opt, optionButton);
            }

        }

        #endregion

        #region === Instantiate Methods ===

        private void InstantiateOption(string text, Object button)
        {
            GameObject newObj = (GameObject) GameObject.Instantiate(button, this.MenuContent);
            newObj.GetComponentInChildren<Text>().text = text;

            newObj.GetComponent<Button>().onClick.AddListener(delegate { this.Crontroller.AddBoardItem(text); });

            if (this.Crontroller.Creator)
            {
                Object availableButton = Resources.Load(BUTTONS_PATH + "AvailableButton");
                GameObject.Instantiate(availableButton, newObj.transform);
            }
        }

        #endregion

    }
}