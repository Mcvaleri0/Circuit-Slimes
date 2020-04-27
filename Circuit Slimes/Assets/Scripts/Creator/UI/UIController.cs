using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Puzzle;
using Creator.Mode;
using Creator.Editor;
using Creator.Selection;
using Creator.UI.ModeUI;



namespace Creator.UI
{
    public class UIController
    {
        #region /* Attributes */

        private ModeUI.ModeUI UIMode { get; set; }

        #endregion



        #region === Init Methods ===

        public UIController(Transform canvas, Mode.Mode mode, PuzzleController controller, 
            PuzzleEditor editor, SelectionSystem selection)
        {
            mode.InitializeUI(this, canvas, controller, editor, selection);
        }


        public void Initialize(Mode.Editor mode, Transform canvas, PuzzleController controller, 
            PuzzleEditor editor, SelectionSystem selection)
        {
            this.UIMode = new EditorUI(canvas, controller, editor, selection, mode);
        }


        public void Initialize(Player mode, Transform canvas, PuzzleController controller, 
            PuzzleEditor editor, SelectionSystem selection)
        {
            this.UIMode = new PlayerUI(canvas, controller, editor, selection, mode);
        }

        #endregion


        #region === Update Info Methods ===

        public void UpdateUI(PuzzleEditor editor, SelectionSystem selection)
        {
            this.UIMode.UpdateInfo(editor, selection);
        }

        #endregion
    }

}
