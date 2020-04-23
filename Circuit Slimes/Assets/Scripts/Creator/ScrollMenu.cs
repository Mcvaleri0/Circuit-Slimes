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

        private Object OptionButton { get; set; }
        private Object AvailableButton { get; set; }

        #endregion

        #region /* Creator Attributes */

        private CreatorController Crontroller { get; set; }

        #endregion 


        public ScrollMenu(CreatorController controller, Transform menu, Transform content, List<string> options, List<string> available)
        {
            this.Crontroller = controller;

            this.Menu = menu;
            this.MenuContent = content;

            this.OptionButton    = Resources.Load(BUTTONS_PATH + "OptionButton");
            this.AvailableButton = Resources.Load(BUTTONS_PATH + "AvailableButton");

            this.Initialize(options, available);
        }


        #region === Initialization Methods === 

        private void Initialize(List<string> options, List<string> available)
        {
            this.ResizeMenu();
            this.PopulateContent(options, available);
        }

        #endregion

        #region === Menu Manipulation Methods ===

        private void ResizeMenu()
        {
            RectTransform rect = this.Menu.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(Screen.width, Screen.height / 4);
            rect.anchoredPosition = new Vector2(0, -3 * Screen.height / 8);
        }

        private void PopulateContent(List<string> options, List<string> available)
        {
            foreach (string opt in options)
            {
                this.InstantiateOption(opt, available);
            }

        }

        private void ClearMenu()
        {
            foreach(Transform child in this.MenuContent)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public void UpdateContent(List<string> newOptions, List<string> available)
        {
            this.ClearMenu();

            this.PopulateContent(newOptions, available);
        }

        #endregion

        #region === Instantiate Methods ===

        private void InstantiateOption(string text, List<string> available)
        {
            GameObject newObj = (GameObject) GameObject.Instantiate(this.OptionButton, this.MenuContent);
            newObj.GetComponentInChildren<Text>().text = text;

            newObj.GetComponent<Button>().onClick.AddListener(delegate { this.Crontroller.AddBoardItem(text); });

            if (this.Crontroller.Creator)
            {
                GameObject avlBtnObj   = (GameObject) GameObject.Instantiate(this.AvailableButton, newObj.transform);
                AvailableButton avlBtnScrp = avlBtnObj.GetComponent<AvailableButton>();

                avlBtnScrp.Initialize(this.Crontroller, text, available.Contains(text));
            }
        }

        #endregion

    }
}