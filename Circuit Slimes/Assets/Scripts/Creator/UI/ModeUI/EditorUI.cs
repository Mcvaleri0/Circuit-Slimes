using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Game;
using Level;
using Puzzle;
using Creator.Editor;
using Creator.Selection;



namespace Creator.UI.ModeUI
{
    public class EditorUI : ModeUI
    {
        #region /* Scroll Menu Attibutes */

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
            #if UNITY_EDITOR
                RectTransform saveRect = base.SaveButton.GetComponent<RectTransform>();

                saveRect.pivot = new Vector2(1, 1);
                float x = -30; //x margin
                float y = -30; //y margin

                saveRect.anchoredPosition = new Vector2(x, y);

                base.SaveButton.GetComponent<Button>().onClick.AddListener(delegate { this.Controller.SaveLevel(); });
            #else
                 base.SaveButton.gameObject.SetActive(false);
            #endif
        }

        #endregion


        #region === Options Methods ===

        override public List<string> MenuOptions()
        {
            if (this.AllItems == null)
            {
                this.AllItems = FileHelper.GetFileList(FileHelper.ITEMS_PATH);
            }

            return this.AllItems;
        }

        #endregion

    }
}
