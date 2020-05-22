using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Game;
using Puzzle;
using Creator.Editor;
using Creator.Selection;



namespace Creator.UI.ModeUI
{
    public class PlayerUI : ModeUI
    {
        #region === Init Methods ===

        public PlayerUI(GameController controller, PuzzleEditor editor, SelectionSystem selection,
            Mode.Mode mode, Transform canvas) : 
                base(controller, editor, selection, mode, canvas) { }

        #endregion


        #region === Buttons Methods ===

        override public void InitializeSave()
        {
            base.SaveButton.gameObject.SetActive(false);
        }
        
        #endregion


        #region === Options Methods ===

        override public List<string> MenuOptions()
        {
            //return this.Editor.Resources();
            return this.Editor.Permissions();
        }


        public override bool AbleToEditOptions()
        {
            return false;
        }

        #endregion

    }
}
