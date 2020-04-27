using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Puzzle;
using Creator.Editor;
using Creator.Selection;



namespace Creator.UI.ModeUI
{
    public abstract class ModeUI
    {
        #region /* Attributes */

        private Mode.Mode Mode { get; set; }

        public Transform SaveButton { get; private set; }

        private ScrollMenu ScrollMenu { get; set; }

        #endregion



        #region === Init Methods ===

        public ModeUI(Transform canvas, PuzzleController controller, PuzzleEditor editor, 
            SelectionSystem selection, Mode.Mode mode)
        {
            this.Mode = mode;
            this.SaveButton = canvas.Find("Save Button");

            this.Initialize(canvas, controller, editor, selection);
        }


        private void Initialize(Transform canvas, PuzzleController controller, PuzzleEditor editor, SelectionSystem selection)
        {
            this.InitializeSaveButton(controller);
            this.InitializeScrollMenu(canvas, editor, selection);
        }

        #endregion


        #region === Update Info Methods ===

        public void UpdateInfo(PuzzleEditor editor, SelectionSystem selection)
        {
            this.ScrollMenu.UpdateContent(this.MenuOptions(editor), editor.Permissions(), 
                editor, selection, this.Mode);
        }

        #endregion


        #region === Buttons Methods ===

        public abstract void InitializeSaveButton(PuzzleController controller);

        #endregion


        #region === Scroll Menu Methods ===

        private void InitializeScrollMenu(Transform canvas, PuzzleEditor editor, SelectionSystem selection)
        {
            Transform menu = canvas.Find("Scroll Menu");

            this.ScrollMenu = new ScrollMenu(menu, this.MenuOptions(editor), editor.Permissions(), 
                                    editor, selection, this.Mode);
        }

        public abstract List<string> MenuOptions(PuzzleEditor editor);

        #endregion

    }
}
