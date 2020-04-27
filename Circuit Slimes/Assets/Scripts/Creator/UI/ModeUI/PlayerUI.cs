using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Puzzle;
using Creator.Editor;
using Creator.Selection;



namespace Creator.UI.ModeUI
{
    public class PlayerUI : ModeUI
    {
        #region === Init Methods ===

        public PlayerUI(Transform canvas, PuzzleController controller, PuzzleEditor editor,
            SelectionSystem selection, Mode.Mode mode) : 
                base(canvas, controller, editor, selection, mode) { }

        #endregion


        #region === Buttons Methods ===

        override public void InitializeSaveButton(PuzzleController controller)
        {
            base.SaveButton.gameObject.SetActive(false);
        }

        #endregion


        #region === Scroll Menu Methods ===

        override public List<string> MenuOptions(PuzzleEditor editor)
        {
            return editor.Permissions();
        }

        #endregion

    }
}
