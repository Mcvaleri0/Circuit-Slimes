using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Game;
using Puzzle;
using Creator.Editor;
using Creator.Selection;



namespace Creator.UI.ModeUI
{
    public class EditorUI : ModeUI
    {
        #region /* Scroll Menu Attibutes */

        private const string ITEMS_PATH = "Prefabs/Board Items";

        private List<string> AllItems { get; set; }

        #endregion


        #region === Init Methods ===

        public EditorUI(GameController controller, PuzzleEditor editor, SelectionSystem selection, 
            Mode.Mode mode, Transform canvas) : 
                base(controller, editor, selection, mode, canvas) { }

        #endregion


        #region === Buttons Methods ===

        override public void InitializeSave()
        {
            RectTransform saveRect = base.SaveButton.GetComponent<RectTransform>();

            saveRect.pivot = new Vector2(1, 1);
            float x = -30; //x margin
            float y = -30; //y margin

            saveRect.anchoredPosition = new Vector2(x, y);

            // add click listener
            int level = this.Controller.CurrentLevel();
            base.SaveButton.GetComponent<Button>().onClick.AddListener(delegate { this.Controller.SaveLevel(level); });
        }


        override public void InitializeItems()
        {
            RectTransform rect = base.ItemsButton.GetComponent<RectTransform>();

            rect.pivot = new Vector2(1, 0);
            float x = -30; //x margin
            float y =  95; //y margin

            rect.anchoredPosition = new Vector2(x, y);
        }


        override public void InitializeResources()
        {
            RectTransform rect = base.ResourcesButton.GetComponent<RectTransform>();

            rect.pivot = new Vector2(1, 0);
            float x = -30; //x margin
            float y =  30; //y margin

            rect.anchoredPosition = new Vector2(x, y);
        }

        #endregion


        #region === Scroll Menu Methods ===

        override public List<string> MenuOptions()
        {
            if (this.AllItems == null)
            {
                Object[] prefabs = Resources.LoadAll(ITEMS_PATH);
                this.AllItems = new List<string>();

                foreach (Object prefab in prefabs)
                {
                    this.AllItems.Add(prefab.name);
                }
            }

            return this.AllItems;
        }

        #endregion

    }
}
