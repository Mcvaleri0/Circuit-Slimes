using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Game;
using Puzzle;
using Creator.Editor;
using Creator.Selection;
using Creator.UI.ModeUI;



namespace Creator.UI
{
    public class UIController
    {
        #region /* Creator Sub-Components */

        private SelectionSystem Selection { get; set; }
        private Mode.Mode Mode { get; set; }

        #endregion


        #region /* Puzzle Attributes */

        private PuzzleEditor Editor { get; set; }
        private GameController Controller { get; set; }

        #endregion


        #region /* Attributes */

        private ModeUI.ModeUI UIMode { get; set; }

        #endregion



        #region === Init Methods ===

        public UIController(PuzzleEditor editor, SelectionSystem selection, Mode.Mode mode,
            GameController controller, Transform canvas)
        {
            this.Editor     = editor;
            this.Selection  = selection;
            this.Mode       = mode;
            this.Controller = controller;

            this.Initialize(canvas);
        }


        public void Initialize(Transform canvas)
        {
            if (this.Mode is Mode.Editor)
            {
                this.UIMode = new EditorUI(this.Controller, this.Editor, this.Selection, this.Mode, canvas);
            }
            else
            {
                this.UIMode = new PlayerUI(this.Controller, this.Editor, this.Selection, this.Mode, canvas);
            }
        }

        #endregion


        #region === Update Info Methods ===

        public void UpdateUI()
        {
            this.UIMode.UpdateInfo();
        }

        #endregion
    }

}
