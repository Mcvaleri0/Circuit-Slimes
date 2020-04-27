using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Puzzle;
using Creator.UI;
using Creator.Editor;
using Creator.Selection;



namespace Creator.Mode
{
    public abstract class Mode
    {
        #region === Init Methods ===

        public abstract void InitializeUI(UIController UIController, Transform canvas,
            PuzzleController puzzleController, PuzzleEditor editor, SelectionSystem selection);

        #endregion


        #region === Select Methods ===

        public abstract void DefineSelectableList(PuzzleEditor editor, SelectionSystem selection);

        #endregion

    }
}
