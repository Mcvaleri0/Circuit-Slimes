using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Puzzle;
using Creator.Editor;
using Creator.Selection;
using Creator.UI.Buttons;



namespace Creator.UI
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


        #region /* Creator Sub-Components */

        private PuzzleEditor Editor { get; set; }
        private SelectionSystem Selection { get; set; }
        private Mode.Mode Mode { get; set; }

        #endregion



        #region === Initialization Methods === 

        public ScrollMenu(PuzzleEditor editor, SelectionSystem selection, Mode.Mode mode,
            Transform menu, List<string> options, List<string> available)
        {
            this.Editor    = editor;
            this.Selection = selection;
            this.Mode      = mode;

            Transform content = menu.Find("Viewport").Find("Content");

            this.Menu = menu;
            this.MenuContent = content;

            this.OptionButton    = Resources.Load(BUTTONS_PATH + "OptionButton");
            this.AvailableButton = Resources.Load(BUTTONS_PATH + "AvailableButton");

            this.Initialize(options, available);
        }


        private void Initialize(List<string> options, List<string> available)
        {
            this.ResizeMenu();
            this.PopulateContent(options, available);
        }

        #endregion


        #region === Menu Methods ===

        private void ResizeMenu()
        {
            RectTransform rect = this.Menu.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(Screen.width / 2, Screen.height / 4);
            rect.anchoredPosition = new Vector2(0, -3 * Screen.height / 8);
        }


        private void PopulateContent(List<string> options, List<string> available)
        {
            foreach (string opt in options)
            {
                this.InstantiateOption(opt, available);
            }
        }


        private void InstantiateOption(string text, List<string> available)
        {
            GameObject newObj = (GameObject)GameObject.Instantiate(this.OptionButton, this.MenuContent);
            newObj.GetComponentInChildren<Text>().text = text;

            OptionButton optionButton = newObj.GetComponent<OptionButton>();
            optionButton.Initialize(this.Editor, text);

            //newObj.GetComponent<Button>().onClick.AddListener(delegate { optionButton.Select(); });
            
            if (this.Mode is Mode.Editor)
            {
                GameObject avlBtnObj = (GameObject)GameObject.Instantiate(this.AvailableButton, newObj.transform);
                AvailableButton avlBtnScrp = avlBtnObj.GetComponent<AvailableButton>();

                avlBtnScrp.Initialize(this.Editor, text, available.Contains(text));
            }
        }


        public void UpdateContent(List<string> newOptions, List<string> available)
        {
            this.ClearMenu();

            this.PopulateContent(newOptions, available);
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