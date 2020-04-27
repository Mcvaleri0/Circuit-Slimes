using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Puzzle;
using Creator.UI;
using Creator.Editor;
using Creator.Selection;



namespace Creator.Mode
{
    public class Editor : Mode
    {
        #region === Init Methods ===

        override public void InitializeUI(UIController UIController, Transform canvas,
            PuzzleController puzzleController, PuzzleEditor editor, SelectionSystem selection)
        {
            UIController.Initialize(this, canvas, puzzleController, editor, selection);
        }

        #endregion


        #region === Select Methods ===

        override public void DefineSelectableList(PuzzleEditor editor, SelectionSystem selection)
        {
            selection.WhiteListAllItens(editor.PiecesTransform(), editor.TilesTransform());
        }

        #endregion

    }
}
