﻿using System.Collections;
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

        public PlayerUI(PuzzleController controller, PuzzleEditor editor, SelectionSystem selection,
            Mode.Mode mode, Transform canvas) : 
                base(controller, editor, selection, mode, canvas) { }

        #endregion


        #region === Buttons Methods ===

        override public void InitializeSaveButton()
        {
            base.SaveButton.gameObject.SetActive(false);
        }

        #endregion


        #region === Scroll Menu Methods ===

        override public List<string> MenuOptions()
        {
            return this.Editor.Permissions();
        }

        #endregion

    }
}