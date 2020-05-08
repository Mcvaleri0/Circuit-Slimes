using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Game;
using Puzzle;
using Creator.Editor;
using Creator.Selection;



namespace Creator.UI.ModeUI
{

    public abstract class ModeUI
    {
        #region /* Creator Sub-Components */

        private SelectionSystem Selection { get; set; }
        private Mode.Mode Mode { get; set; }

        #endregion


        #region /* Puzzle Attributes */

        protected GameController Controller { get; private set; }
        protected PuzzleEditor Editor { get; set; }

        #endregion


        #region /* UI Attributes */

        public Transform SaveButton { get; private set; }

        private ScrollMenu ScrollMenu { get; set; }

        #endregion



        #region === Init Methods ===

        public ModeUI(GameController controller, PuzzleEditor editor, SelectionSystem selection, 
            Mode.Mode mode, Transform canvas)
        {
            this.Controller = controller;
            this.Editor     = editor;
            this.Selection  = selection;
            this.Mode       = mode;

            this.SaveButton = canvas.Find("Save Button");

            this.Initialize(canvas);
        }


        private void Initialize(Transform canvas)
        {
            this.InitializeSaveButton();
            this.InitializeScrollMenu(canvas);
        }

        #endregion


        #region === Update Info Methods ===

        public void UpdateInfo()
        {
            this.ScrollMenu.UpdateContent(this.MenuOptions(), this.Editor.Permissions());
        }

        #endregion


        #region === Buttons Methods ===

        public abstract void InitializeSaveButton();

        #endregion


        #region === Scroll Menu Methods ===

        private void InitializeScrollMenu(Transform canvas)
        {
            Transform menu = canvas.Find("Scroll Menu");

            this.ScrollMenu = new ScrollMenu(this.Editor, this.Selection, this.Mode,
                                    menu, this.MenuOptions(), this.Editor.Permissions());
        }

        public abstract List<string> MenuOptions();

        #endregion

    }
}
