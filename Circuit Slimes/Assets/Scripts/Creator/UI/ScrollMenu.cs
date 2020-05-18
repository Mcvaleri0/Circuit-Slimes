using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Level;
using Puzzle;
using Creator.Editor;
using Creator.Selection;
using Creator.UI.Buttons;



namespace Creator.UI
{
    public class ScrollMenu
    {
        #region /* UI Elements */

        private const string BUTTONS_PATH = "Prefabs/";

        private Transform Menu { get; set; }
        private Transform MenuContent { get; set; }

        private Object OptionButton { get; set; }
        private Object AvailableButton { get; set; }

        #endregion


        #region /* Creator Sub-Components */

        private PuzzleEditor Editor { get; set; }
        private SelectionSystem Selection { get; set; }
        private Mode.Mode Mode { get; set; }

        #endregion



        #region === Initialization Methods === 

        public ScrollMenu(PuzzleEditor editor, SelectionSystem selection, Mode.Mode mode,
            Transform menu, List<string> options)
        {
            this.Editor    = editor;
            this.Selection = selection;
            this.Mode      = mode;

            Transform content = menu.Find("Viewport").Find("Content");

            this.Menu = menu;
            this.MenuContent = content;

            this.OptionButton    = Resources.Load(FileHelper.BUTTON_PATH);
            this.AvailableButton = Resources.Load(BUTTONS_PATH + "Creator/AvailableButton");

            this.Initialize(options, this.OptionButton);
        }


        private void Initialize(List<string> options, Object button)
        {
            this.ResizeMenu();
            this.PopulateContent(options, button);
        }

        #endregion


        #region === Menu Methods ===

        private void ResizeMenu()
        {
            RectTransform rect = this.Menu.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(Screen.width / 2, Screen.height / 4);
            rect.anchoredPosition = new Vector2(0, -3 * Screen.height / 8);
        }


        private void PopulateContent(List<string> options, Object button)
        {
            foreach (string opt in options)
            {
                this.InstantiateOption(opt, button);
            }
        }


        private void InstantiateOption(string text, Object button)
        {
            GameObject newObj = (GameObject)GameObject.Instantiate(button, this.MenuContent);
            newObj.GetComponentInChildren<Text>().text = text;

            OptionButton optionButton = newObj.AddComponent<OptionButton>();
            optionButton.Initialize(this.Editor, text);
            
            if (this.Mode is Mode.Editor)
            {
                GameObject avlBtnObj = (GameObject)GameObject.Instantiate(this.AvailableButton, newObj.transform);
                AvailableButton avlBtnScrp = avlBtnObj.GetComponent<AvailableButton>();

                avlBtnScrp.Initialize(this.Editor, text, available.Contains(text));
            }
        }


        public void UpdateContent(List<string> newOptions)
        {
            this.ClearMenu();

            this.PopulateContent(newOptions, this.OptionButton);
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