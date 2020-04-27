using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Puzzle;
using Creator.Editor;
using Creator.Selection;



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



        public ScrollMenu(Transform menu, List<string> options, List<string> available, 
            PuzzleEditor editor, SelectionSystem selection, Mode.Mode mode)
        {           
            Transform content = menu.Find("Viewport").Find("Content");

            this.Menu = menu;
            this.MenuContent = content;

            this.OptionButton    = Resources.Load(BUTTONS_PATH + "OptionButton");
            this.AvailableButton = Resources.Load(BUTTONS_PATH + "AvailableButton");

            this.Initialize(options, available, editor, selection, mode);
        }


        #region === Initialization Methods === 

        private void Initialize(List<string> options, List<string> available, PuzzleEditor editor,
            SelectionSystem selection, Mode.Mode mode)
        {
            this.ResizeMenu();
            this.PopulateContent(options, available, editor, selection, mode);
        }

        #endregion


        #region === Menu Methods ===

        private void ResizeMenu()
        {
            RectTransform rect = this.Menu.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(Screen.width, Screen.height / 4);
            rect.anchoredPosition = new Vector2(0, -3 * Screen.height / 8);
        }

        private void PopulateContent(List<string> options, List<string> available, PuzzleEditor editor,
            SelectionSystem selection, Mode.Mode mode)
        {
            foreach (string opt in options)
            {
                this.InstantiateOption(opt, available, editor, selection, mode);
            }
        }

        private void InstantiateOption(string text, List<string> available, PuzzleEditor editor, 
            SelectionSystem selection, Mode.Mode mode)
        {
            GameObject newObj = (GameObject)GameObject.Instantiate(this.OptionButton, this.MenuContent);
            newObj.GetComponentInChildren<Text>().text = text;

            newObj.GetComponent<Button>().onClick.AddListener(delegate { editor.AddItem(text, selection); });

            if (mode is Mode.Editor)
            {
                GameObject avlBtnObj = (GameObject)GameObject.Instantiate(this.AvailableButton, newObj.transform);
                AvailableButton avlBtnScrp = avlBtnObj.GetComponent<AvailableButton>();

                avlBtnScrp.Initialize(editor, text, available.Contains(text));
            }
        }

        public void UpdateContent(List<string> newOptions, List<string> available, 
            PuzzleEditor editor, SelectionSystem selection, Mode.Mode mode)
        {
            this.ClearMenu();

            this.PopulateContent(newOptions, available, editor, selection, mode);
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