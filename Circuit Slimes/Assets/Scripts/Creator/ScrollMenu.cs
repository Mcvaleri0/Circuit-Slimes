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

        private const string ITEMS_PATH  = "Prefabs/Board Items";
        private const string BUTTON_PATH = "Prefabs/Creator/Button";

        #endregion

        #region /* UI Elements */

        private Transform Menu { get; set; }
        private Transform MenuContent { get; set; }

        #endregion

        #region /* Creator Attributes */

        private CreatorController Crontroller { get; set; }

        #endregion 


        public ScrollMenu(CreatorController controller, Transform menu, Transform content)
        {
            this.Crontroller = controller;

            this.Menu = menu;
            this.MenuContent = content;

            this.Initialize();
        }


        #region === Initialization Methods === 

        private void Initialize()
        {
            this.ResizeMenu();
            this.PopulateContent();
        }

        private void ResizeMenu()
        {
            RectTransform rect = this.Menu.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(Screen.width, Screen.height / 4);
            rect.anchoredPosition = new Vector2(0, -3 * Screen.height / 8);
        }

        private void PopulateContent()
        {
            Object[] icons = Resources.LoadAll(ITEMS_PATH);
            Object button  = Resources.Load(BUTTON_PATH);

            foreach (Object icon in icons)
            {
                this.InstantiateOption(icon.name, button);
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