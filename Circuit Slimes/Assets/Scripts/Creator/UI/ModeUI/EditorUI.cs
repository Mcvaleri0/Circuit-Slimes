using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        public EditorUI(Transform canvas, PuzzleController controller, PuzzleEditor editor,
            SelectionSystem selection, Mode.Mode mode) : 
                base(canvas, controller, editor, selection, mode) { }

        #endregion


        #region === Buttons Methods ===

        override public void InitializeSaveButton(PuzzleController controller)
        {
            RectTransform saveRect = base.SaveButton.GetComponent<RectTransform>();

            saveRect.pivot = new Vector2(1, 1);
            float x = -30; //x margin
            float y = -30; //y margin

            saveRect.anchoredPosition = new Vector2(x, y);

            // add click listener
            int level = controller.CurrentLevel;
            base.SaveButton.GetComponent<Button>().onClick.AddListener(delegate { controller.SaveLevel(level); });
        }

        #endregion


        #region === Scroll Menu Methods ===

        override public List<string> MenuOptions(PuzzleEditor editor)
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
