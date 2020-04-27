using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Puzzle;
using Creator.Editor;
using Creator.Selection;



namespace Creator
{
    public class ScrollMenu
    {
        #region /* UI Elements */

        private const string BUTTONS_PATH = "Prefabs/Creator/";

        private Transform Menu { get; set; }
        private Transform MenuContent { get; set; }

        private Object OptionButton { get; set; }
        private Object AvailableButton { get; set; }

        #endregion

        #region /* Creator Attributes */

        private CreatorController Crontroller { get; set; }

        #endregion 



        public ScrollMenu(CreatorController controller, Transform menu, Transform content,
            List<string> options, List<string> available, PuzzleEditor editor, SelectionSystem selection)
        {
            this.Crontroller = controller;

            this.Menu = menu;
            this.MenuContent = content;

            this.OptionButton    = Resources.Load(BUTTONS_PATH + "OptionButton");
            this.AvailableButton = Resources.Load(BUTTONS_PATH + "AvailableButton");

            this.Initialize(options, available, editor, selection);
        }


        #region === Initialization Methods === 

        private void Initialize(List<string> options, List<string> available, PuzzleEditor editor, SelectionSystem selection)
        {
            this.ResizeMenu();
            this.PopulateContent(options, available, editor, selection);
        }

        #endregion


        #region === Menu Methods ===

        private void ResizeMenu()
        {
            RectTransform rect = this.Menu.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(Screen.width, Screen.height / 4);
            rect.anchoredPosition = new Vector2(0, -3 * Screen.height / 8);
        }

        private void PopulateContent(List<string> options, List<string> available, PuzzleEditor editor, SelectionSystem selection)
        {
            foreach (string opt in options)
            {
                this.InstantiateOption(opt, available, editor, selection);
            }
        }

        private void InstantiateOption(string text, List<string> available, PuzzleEditor editor, SelectionSystem selection)
        {
            GameObject newObj = (GameObject)GameObject.Instantiate(this.OptionButton, this.MenuContent);
            newObj.GetComponentInChildren<Text>().text = text;

            newObj.GetComponent<Button>().onClick.AddListener(delegate { editor.AddItem(text, selection); });

            if (this.Crontroller.Creator)
            {
                GameObject avlBtnObj = (GameObject)GameObject.Instantiate(this.AvailableButton, newObj.transform);
                AvailableButton avlBtnScrp = avlBtnObj.GetComponent<AvailableButton>();

                avlBtnScrp.Initialize(editor, text, available.Contains(text));
            }
        }

        public void UpdateContent(List<string> newOptions, List<string> available, PuzzleEditor editor, SelectionSystem selection)
        {
            this.ClearMenu();

            this.PopulateContent(newOptions, available, editor, selection);
        }

        private void ClearMenu()
        {
            foreach (Transform child in this.MenuContent)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        #endregion

    }
}